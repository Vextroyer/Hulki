/*
This classes represents the nodes of the abstract syntax tree.
The tree is produced by the parser and then consumed by the interpreter.
*/

namespace Hulk;

/*
Base class for expressions
*/
abstract class Expr{
    public abstract R Accept<R> (Visitor<R> visitor);//Visitor design pattern.
}
/*
Classes that handle expressions should implement this interface.
This work with the Accept  method on Expr class.
With this scheme the class that implements the interface can have a method
receive a object of type Expr, call its Accept method, wich then will call back
the correct method for its type, Literal, Binary , etc.
*/
interface Visitor<R>{
    R VisitCallExpr(CallExpr expr);
    R VisitFunctionExpr(FunctionExpr expr);
    R VisitVariableExpr(VariableExpr expr);
    R VisitAssignmentExpr(AssignmentExpr expr);
    R VisitLetInExpr(LetInExpr expr);
    R VisitConditionalExpr(ConditionalExpr expr);
    R VisitBinaryExpr(BinaryExpr expr);
    R VisitLiteralExpr(LiteralExpr expr);
    R VisitUnaryExpr(UnaryExpr expr);
}

//Represents string and number literals and also boolean values true and false.
class LiteralExpr : Expr{
    public object Value {get; private set;}

    public LiteralExpr(object value){
        this.Value = value;
    }

    public override R Accept<R>(Visitor<R> visitor)
    {
        return visitor.VisitLiteralExpr(this);
    }
}

//Represents unary operators like (! -)
class UnaryExpr : Expr{
    public Token Operation{get; private set;}
    public Expr Expression{get; private set;}

    public UnaryExpr(Token operation,Expr expr){
        Operation = operation;
        Expression = expr;
    }

    public override R Accept<R>(Visitor<R> visitor)
    {
        return visitor.VisitUnaryExpr(this);
    }
}
//Represents Binary Operators like + - * / ^ @
class BinaryExpr : Expr{
    public Expr Left{get; private set;}
    public Expr Right{get; private set;}

    public Token Operation{get; private set;}

    public BinaryExpr(Expr left,Token operation,Expr right){
        Left = left;
        Operation = operation;
        Right = right;
    }

    public override R Accept<R>(Visitor<R> visitor)
    {
        return visitor.VisitBinaryExpr(this);
    }
}
//Represents an if-else or conditional expression.
class ConditionalExpr : Expr{
    public Expr Condition{get; private set;}
    public Expr IfBranchExpr{get; private set;}
    public Expr ElseBranchExpr{get; private set;}

    //For error printing purposes
    public int IfOffset{get; private set;}
    public int ElseOffset{get; private set;}

    public ConditionalExpr(Expr condition, Expr ifBranchExpr, Expr elseBranchExpr, int ifOffset = -1, int elseOffset = -1){
        Condition = condition;
        IfBranchExpr = ifBranchExpr;
        ElseBranchExpr = elseBranchExpr;
        IfOffset = ifOffset;
        ElseOffset = elseOffset;
    }

    public override R Accept<R>(Visitor<R> visitor)
    {
        return visitor.VisitConditionalExpr(this);
    }
}
//Represents a let - in expression
class LetInExpr : Expr{
    public List<AssignmentExpr> Assignments {get; private set;}
    public Expr InBranchExpr {get; private set;}

    public LetInExpr(List<AssignmentExpr> assignments, Expr inBranchExpr){
        Assignments = assignments;
        InBranchExpr = inBranchExpr;
    }

    public override R Accept<R>(Visitor<R> visitor)
    {
        return visitor.VisitLetInExpr(this);
    }
}
//Represents an assigment expression. Can only be used in a let-in expression.
class AssignmentExpr : Expr{
    public Token Identifier{get; private set;}
    public Expr RValue{get; private set;}

    public AssignmentExpr(Token identifier, Expr rvalue){
        Identifier = identifier;
        RValue = rvalue;
    }

    public override R Accept<R>(Visitor<R> visitor)
    {
        return visitor.VisitAssignmentExpr(this);
    }
}
//Represents an identifier, a variable.
class VariableExpr : Expr{
    public Token Identifier {get; private set;}

    public VariableExpr(Token identifier){
        Identifier = identifier;
    }

    public override R Accept<R>(Visitor<R> visitor)
    {
        return visitor.VisitVariableExpr(this);
    }
}
//Represents a function declaration
class FunctionExpr : Expr{
    public Token Identifier{get; private set;}
    public Expr Body {get; private set;}
    //Arguments of the function
    public List<Token> Args{get; private set;}
    //Amount of arguments
    public int Arity{
        get{
            return Args.Count;
        }
    }

    public FunctionExpr(Token identifier,List<Token> args,Expr body){
        Identifier = identifier;
        Args = args;
        Body = body;
    }

    public override R Accept<R>(Visitor<R> visitor)
    {
        return visitor.VisitFunctionExpr(this);
    }
    //A method for randomizing the name of the arguments.
    public void RandomizeArgs(){
        for(int i = 0;i<Args.Count;++i){
            Token actual = Args[i];
            Token randomized = new Token(actual.Type,actual.Lexeme + actual.GetHashCode(),actual.Literal,actual.Offset);
            Body = new Replacer().Replace(Body,actual,randomized);//Also replace the body
            Args[i] = randomized;
        }
    }
}
//Represents a function call
class CallExpr : Expr{
    public Token Identifier {get; private set;}
    public List<Expr> Parameters{get; private set;}
    //Amount of parameters
    public int Arity{
        get{
            return Parameters.Count;
        }
    }
    public CallExpr(Token identifier, List<Expr> parameters){
        Identifier = identifier;
        Parameters = parameters;        
    }

    public override R Accept<R>(Visitor<R> visitor)
    {
        return visitor.VisitCallExpr(this);
    }
}
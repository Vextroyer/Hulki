/*
Environment for the interpreter.
*/
namespace Hulk;
class Environment{
    private Dictionary<string,List<object>> table;//Associates identifiers to their values. A list is used for support nested declarations.
    /*
    The function representation is as follows:
    A function is uniquely determined by its name and arity.
    Name comes first.
    Arity comes next.
    This is an example of the structure used for storing functions.
    Name                Arity               An example body
    
    Sum --------------- 2 ----------------- a + b
        \-------------- 3 ----------------- a + b + c
    
    Max --------------- 2 ----------------- if(a >= b) a else b
        \-------------- 3 ----------------- let m = Max(a,b) in if(m >= c) m else c
    
    This structure can be extended to support types because types comes after arity in the hieracy.
    */
    private Dictionary<string,Dictionary<int,FunctionExpr>> funcTable;//Represents the functions. 
    public Environment(){
        table = new Dictionary<string,List<object>>();
        funcTable = new Dictionary<string, Dictionary<int, FunctionExpr>>();
        //Put the primitive functions here.
    }

    //Retrieves the value associated with the given identifier.
    public object Get(Token identifier){
        try{
            if(IsFunction(identifier))throw new InterpreterException("Mising '(' after call to function '"+identifier.Lexeme+"'.",identifier.Offset + identifier.Lexeme.Length);
            return table[identifier.Lexeme].Last();
        }catch(KeyNotFoundException){
            throw new InterpreterException("Variable '" + identifier.Lexeme + "' is used but not declared.",identifier.Offset);
        }
    }
    //Associates an identifier with a value.
    public void Set(Token identifier,object value){
        
        //No variable can be named as a declared function, except as an argument to a function. Its explained in the register method the reason of this exception.
        //This could be done in the parser but the interpreter has the suitable methods for doing it.
        if(IsFunction(identifier))throw new InterpreterException("A function name can not be used as the name of a variable.",identifier.Offset);

        if(!table.ContainsKey(identifier.Lexeme))table.Add(identifier.Lexeme,new List<object>());
        table[identifier.Lexeme].Add(value);
    }

    //Remove the last value associated with the identifier and delete it if no values remains associated to it.
    public void Remove(Token identifier){
        table[identifier.Lexeme].RemoveAt(table[identifier.Lexeme].Count - 1);//Removes the last element.
        if(table[identifier.Lexeme].Count == 0)table.Remove(identifier.Lexeme);//Remove the association if there are no more values.
    }
    //Register a function. Functions are globally scoped.
    public void Register(FunctionExpr fun){
        string name = fun.Identifier.Lexeme;
        int arity = fun.Arity;
        
        //Builtin functions can not be redefined.
        if(IsBuiltin(name,arity))throw new InterpreterException($"'{name}' is a built-in function and cannot be redefined.",fun.Identifier.Offset);

        //Randomize the names of the arguments so it becomes unlikely that they collide with a function name, existing or yet to be declared.
        fun.RandomizeArgs();

        if(funcTable.ContainsKey(name)){
            //There is a function with this name
            Dictionary<int,FunctionExpr> arityTable = funcTable[name];
            if(arityTable.ContainsKey(arity)){
                //There is a function with this arity
                throw new InterpreterException($"Cant redeclare function '{name}'. Function redeclaration is not allowed.",fun.Identifier.Offset);
            }else{
                funcTable[name].Add(arity,fun);
            }
        }else{
            Dictionary<int,FunctionExpr> arityTable = new Dictionary<int, FunctionExpr>();
            arityTable.Add(arity,fun);
            funcTable.Add(name,arityTable);
        }
    }
    //Determine if a given identifier can be used like a function.
    public bool IsFunction(Token identifier){
        if(funcTable.ContainsKey(identifier.Lexeme))return true;
        return false;
    }
    public bool IsFunction(Token identifier,int arity){
        if(IsFunction(identifier)){
            return funcTable[identifier.Lexeme].ContainsKey(arity);
        }
        return false;
    }
    //Returns the parameters of the corresponding function.
    public List<Token> GetArguments(string name,int arity){
        return funcTable[name][arity].Args;
    }
    //Returns the body of the corresponding function.
    public Expr GetBody(string name,int arity){
        return funcTable[name][arity].Body;
    }
    //Returns a list of possible aritys for the function.
    public List<int> GetAritys(string funName){
        return funcTable[funName].Keys.ToList();
    }
    //Returns true if the given identifier corresponds to a builtin function
    public bool IsBuiltin(string name,int arity){
        return builtIns.Contains((name,arity));
    }
    private HashSet<(string,int)> builtIns = new HashSet<(string, int)>(){
        ("rand",0),
        ("cos",1),
        ("exp",1),
        ("print",1),
        ("sin",1),
        ("sqrt",1),
        ("log",2)
    };
}
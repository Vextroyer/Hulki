/*
Errors that can arise in the process of interpreting a Hulk expression.
*/
namespace Hulk;

/*
Base class for all Hulk defined exceptions.
*/
abstract class HulkException : Exception{
    abstract public string ErrorType();//Stablish the type of this error : Lexical, Syntactic, Semantic
    abstract public void HandleException();

    //Amount of characters from the begining of the line where the error happened. Its an aproximation of the error ubication.
    public int Ofsset {get;private set;}

    //Initailizes exception class with a message, and the initial position of the error in the line or -1 if such position is to be ignored.
    protected HulkException(string? message="", int offset = -1):base(message){
        this.Ofsset = offset;
    }
}

/*
This kind of exception rise while scanning the source code. They represent
Hulk lexical errors.
*/
class ScannerException : HulkException{
    public ScannerException(string message="", int offset=-1):base(message,offset){}

    public override string ErrorType(){
        return "LEXICAL ERROR";
    }
    public override void HandleException()
    {
        Hulk.Error(this);
        throw this;
    }
}

/*
This kind of exception rise while parsing the tokens produced by the scanner. They represent
Hulk syntactic errors.
*/
class ParserException : HulkException{
    public ParserException(string message="",int offset=-1) : base(message,offset){}

    public override string ErrorType(){
        return "SYNTACTIC ERROR";
    }
    public override void HandleException()
    {
        Hulk.Error(this);
        throw this;
    }
}

/*
This kind of exceptions rise while interpreting the syntax tree generated by the parser. They represent
Hulk semantic errors.
*/
class InterpreterException : HulkException{
    public InterpreterException(string message="", int offset=-1) : base(message,offset){}

    public override string ErrorType(){
        return "SEMANTIC ERROR";
    }
    public override void HandleException()
    {
        Hulk.Error(this);
        throw this;
    }
}
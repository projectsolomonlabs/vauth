namespace Vauth.IO.Json
{
    enum JPathTokenType : byte
    {
        Root,
        Dot,
        LeftBracket,
        RightBracket,
        Asterisk,
        Comma,
        Colon,
        Identifier,
        String,
        Number
    }
}

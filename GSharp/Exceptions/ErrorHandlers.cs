namespace GSharp.Exceptions;

public delegate void ParseErrorHandler(ParseError parseError);
public delegate void ScanErrorHandler(ScanError scanError);
public delegate void NameResolutionErrorHandler(NameResolutionError nameResolutionErrorHandler);
public delegate void ValidationErrorHandler(ValidationError typeValidationError);
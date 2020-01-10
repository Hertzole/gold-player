# Contributing to Gold Player
First of all, I want to thank you so much for considering contributing! It helps a lot and will improve this player controller be a viable free drag-n-drop player controller!

## Getting Started
- Fork the project and get it onto your computer.
- Make your changes to your repository and then commit them.
- When you're happy with your changes, create a pull request [here](https://github.com/Hertzole/gold-player/pulls).
- Write a description of what it does and why you decided to add it.

## Code Format
If you take a look at Gold Player, you may notice a special coding style. I want to have these in place to make it all consistent.

#### Fields 
Fields should always be either private or protected and be lower camel case (`exampleName`) .If the field should be accessible from other files, it should be accompanied by a property. If they should be accessed inside Unity, they need the `[SerializeField]` attribute and should have a default value, even if it's just null, along with a `[Tooltip()]` attribute describing the function and a public property. The tooltip should match corresponding property's XML documentation.  
Example: `private string stringField;`

#### Properties
Properties should be in [Pascal Case](http://wiki.c2.com/?PascalCase) and are often used to give access to private/protected fields. There should be enough spacing like seen in the example. If the property is only used to access a field, it should be on one line. If there's more logic to it, it can be spread out over multiple lines.  
Example: `public string StringProperty { get { return stringField; } set { stringField = value; } }`

#### Consts and Enums
A constant should always be uppercase with underscores as "spaces", except when it's in an enum, where they should be [Pascal Cased](http://wiki.c2.com/?PascalCase). Enums should also always include a number associated with each value.  
Const example: `public const string RUN_BUTTON = "Run";`  
Enum example: `public enum ExampleEnum { OptionOne = 0, OptionTwo = 1, NoConsistency = 2 }`

#### Functions
Functions are also supposed to be [Pascal Case](http://wiki.c2.com/?PascalCase) and nothing else. The parenthesis should be directly next to the function name. The parameters in the function should have space after a comma.  
Example: `public void MyFunction(string stringValue, float floatValue) { }`

#### Comments
Gold Player is *very* documented. Some may even think a bit too documented, but that's because I want someone with very limited programming skills to know what's going on. There should be comments on *most* of the code and they follow a specific format. A space after the two slashes, proper capitalization, and a period in the end. They shouldn't be long lines. If something needs a bit of explaining, split it up in multiple lines.  
Example: `// This is a comment about some code. Neat, huh?`

#### XML Documentation
For most accessible functions/properties there's XML documentation that makes it easier to see what it does inside an IDE that supports it. Properties should have one-liners, except if they need a really big explanation and functions should have it on multiple lines. They also a specific format, like comments, with a space after the three slashes and a space after the beginning and after the end, proper capitalization and a period in the end.  
Property example: `/// <summary> XML documentation for a property. </summary>`  
Function example:  
```csharp
/// <summary>
/// This is an example of documenting a function.
/// </summary>
```

## OnValidate
OnValidate is a Unity "magic function" that gets called when the script's fields are changed in the inspector, so that means it's an editor only function. So it doesn't need to be included in the build. Enclose it in `#if UNITY_EDITOR`, like this:  
```csharp
#if UNITY_EDITOR
private void OnValidate()
{
    // Validation code...
}
#endif
```
It should also always be private EXCEPT when it's in a no-MonoBehaviour class and something else needs to call it.

## Space and Header attributes
Just like OnValidate these are Unity editor only features and don't need to be included in the build. Header should be right above the next field and space should have a space above it and a space below it. Enclose them inside `#if UNITY_EDITOR`.  
Example:  
```csharp
#if UNITY_EDITOR
    [Header("Header")]
#endif
    [SerializeField]
    private string greeting = "Hello";
    
#if UNITY_EDITOR
    [Space]
#endif

    [SerializeField]
    private string target = "World";
```

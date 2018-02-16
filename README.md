# Argumentor - Argument Handler
Command Line Argument processor

<h2>Instructions</h2>

```C#
// Define the parameters you expect by creating a class
internal class Arguments
{
    // For example, to process a mandatory switch like so: -c 100
    [Switch(Key = "c", Description="Number of pings to send")]
    [Mandatory]
    public int Pings { get; set; }

    // To process an optional switch like so: -t yes
    [Switch(Key = "t", Description = "Tengu Maru")]
    public string TenguMaru { get; set; }

    // To add a boolean switch with no value like so: -switch
    [Switch(Key = "switch", Description = "no value")]
    public bool Switch { get; set; }

    // Add individual parameter arguments by name.
    [Parameter(Key = "host", Description = "The name of the host")]
    public string Host { get; set; }

    // Add zero or more arguments (one or more if mandatory) as the last property.
    [Parameter(Key = "...", Description = "Files to process")]
    public List<string> FilesToProcess { get; set; }
}

// Create an instance of the Argument processor
var argumentor = new Argumentor<Arguments>();

// Parse the command line arguments:
var arguments = argumentor.Parse(args);

// To retrieve the value of an argument
arguments.Pings
arguments.Switch

```

If invalid arguments are passed to the parser, an ArgumentException is thrown. The Usage information can be displayed by making a call to

```C#
new UsageFormatter<Arguments>().ToString()
```
```
ERROR Expecting switches: v

ArgumentHandler -c value [-t value] [-u value] -v value [-w value] [-x value] host ...

 c    The number of pings to send
 t    Tengu maru
 u    Umbrella count
 v    Victor is an exceedingly handsome fellow
 w    With enough help, anything is possible
 x    x-rays
 host The name of the host
 ...  files to process
```

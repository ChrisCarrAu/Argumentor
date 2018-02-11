# Argumentor - Argument Handler
Command Line Argument processor

<h2>Instructions</h2>

```C#

// Create an instance of the Argument processor
var argumentor = new Argumentor();

// Add command line switches - give them a description and mark them as mandatory or optional
// For example, to process a switch like so: -c 100
argumentor.AddSwitch("c", "The number of pings to send", Required.Mandatory);
// To process an optional switch like so: -t yes
argumentor.AddSwitch("t", "Tengu maru", Required.Optional);
// To add a switch with no value like so: -switch
argumentor.AddSwitch("switch", "A no-value switch", Required.Optional).HasValue = false;

// Add individual arguments by name
argumentor.AddArgument("host", "The name of the host", Required.Mandatory);

// Add zero or more arguments (one or more if mandatory). These arguments are keyed by their ordinal position.
argumentor.AddArguments("files to process", Required.Mandatory);

// To parse the command line arguments:
var arguments = argumentor.Parse(args);

// The switches can be found in arguments.GetSwitches()
// The parameters can be found in arguments.GetParameters()

// To retrieve the value of an argument
arguments["c"]
arguments["switch"]

```

If invalid arguments are passed to the parser, an ArgumentException is thrown. The Usage information can be displayed by making a call to

```C#
argumentor.Usage("ArgumentHandler")
```

ERROR Expecting switches: v

ArgumentHandler -c value [-t value] [-u value] -v value [-w value] [-x value] host ...
 c    : The number of pings to send
 t    : Tengu maru
 u    : Umbrella count
 v    : Victor is an exceedingly handsome fellow
 w    : With enough help, anything is possible
 x    : x-rays
 host : The name of the host
 ...  : files to process


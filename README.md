# SECW
which documentation tool are we moving forward with? I think it may be best to use Docfx due to its auto gernerative html documents from xml comments instead of the alternatives such as sandcastle (which is a tad bit too old) or doxygen (compatible with diffrent langauges) 

I already installed Docfx on my device using "dotnet tool install -g docfx"
and we can simply comment the desired descriptive sections using // and invoke its use with docfx build which as the document says should create the html file automatically in the _site folder. as for presentation am not too sure if it adds any styling or css but a simple document should suffice.

used command "dotnet list package --include-transitive > packages.txt"
in order to create a file similar to requiremnts, to store all packages and dependancies needed to be installed for this specific application to run.

the database helper is coded in C# and does not include actual required tables but examples of previously made code snippets and comments.

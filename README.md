SpellUnit is a simple set of tools of incorporating spelling and grammar checks into your .NET unit tests.  Currently this consists of:
-	Core unit test running and configuration engine
-	String extractor for ServiceStack related documentation attributes
-	Integration of spellchecking through NHunspell
-	Trivial grammar validation
-	Command line tool to list the validation failures of a given assembly
Since spell unit uses MEF to import both string extractors and validators both are ridiculously easy to integrate with the library.
For an example of how to use the library take a look at the “SpellUnitBasicTests” that demonstrates how to incorporate the library into your unit tests.

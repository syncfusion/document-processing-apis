#addin nuget:?package=Cake.FileHelpers&version=3.3.0
// Target - The task you want to start. Runs the Default task if not specified.
var target = Argument("Target", "Default");
            

// Configuration - The build configuration (Debug/Release) to use.
var configuration = 
    HasArgument("Configuration") 
        ? Argument<string>("Configuration") 
        : EnvironmentVariable("Configuration") ?? "Release";

// The build number to use in the version number of the built NuGet packages.
var buildNumber =
    HasArgument("BuildNumber") ? Argument<int>("BuildNumber") :
    AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Build.Number :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Build.BuildNumber :
    EnvironmentVariable("BuildNumber") != null ? int.Parse(EnvironmentVariable("BuildNumber")) : 0;

// Packages version in format major.minor.patch
var version = HasArgument("ShortVersion") ? Argument<string>("ShortVersion") : EnvironmentVariable("ShortVersion");
version = !string.IsNullOrWhiteSpace(version) ? version : "1.0.0";
var assemblyVersion = $"{version}.{buildNumber}";

// Text suffix of the package version
var versionSuffix = HasArgument("VersionSuffix") ? Argument<string>("VersionSuffix") : EnvironmentVariable("VersionSuffix");
var packageVersion = version + (!string.IsNullOrWhiteSpace(versionSuffix) ? $"-{versionSuffix}-build{buildNumber}" : "");
 
// A directory path to an Artifacts directory.
var artifactsDirectory = MakeAbsolute(Directory("./artifacts"));
var fileName="documentprocessingservice";
var cireports = Argument("cireports","../cireports");
var FXReportDir = cireports + "/fxcopviolation";
var fxCopReport = FXReportDir + "/FXCopViolations.txt";
var waringsFolder=cireports + "/warnings/";
var errorlogsFolder=cireports + "/errorlogs/";
// Deletes the contents of the Artifacts folder if it should contain anything from a previous build.
Task("Clean").Does(() => CleanDirectory(artifactsDirectory)).OnError(exception =>
{
	FileWriteText("../cakelog.txt", "Error occur while build the project:" + exception.ToString());
	throw new Exception("Cake process Failed on Compilation Task");
});

 
// Find all csproj projects and build them using the build configuration specified as an argument.
 Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
    {
		if (DirectoryExists(waringsFolder))
		{
		 DeleteDirectory(waringsFolder, new DeleteDirectorySettings {
		Recursive = true,
		Force = true
		});
		}	
		if (DirectoryExists(errorlogsFolder))
		{
		 DeleteDirectory(errorlogsFolder, new DeleteDirectorySettings {
		Recursive = true,
		Force = true
		});
		}
	  
      	var projectCount=0;	
        var projects = GetFiles("../src/**/*.csproj");
        foreach(var project in projects)
        {
		if(!project.FullPath.Contains("Database")){
		 var settings = new DotNetCoreMSBuildSettings();
		  settings.FileLoggers.Add(new MSBuildFileLoggerSettings(){
            LogFile = waringsFolder+fileName+"_"+projectCount+".txt",
            SummaryOutputLevel = MSBuildLoggerOutputLevel.WarningsOnly
        });
		        settings.FileLoggers.Add(new MSBuildFileLoggerSettings(){
            LogFile = errorlogsFolder+fileName+"_"+projectCount+".txt",
            SummaryOutputLevel = MSBuildLoggerOutputLevel.ErrorsOnly
        });
		Information("Project build: " + project);
            DotNetCoreBuild(
                project.GetDirectory().FullPath,
                new DotNetCoreBuildSettings()
                {
                    Configuration = configuration,
					MSBuildSettings=settings,
                    ArgumentCustomization = args => args
                        .Append($"/p:Version={version}")
                        .Append($"/p:AssemblyVersion={assemblyVersion}")
                });
				    projectCount++;
				}
        }
		
    }).OnError(exception =>
{
	FileWriteText("../cakelog.txt", "Error occur while build the project:" + exception.ToString());
	throw new Exception("Cake process Failed on Compilation Task");
});
Task("codeviolation")
	.IsDependentOn("Get-Fx-cop-Reports")
    .Does(() =>
{
}).OnError(exception =>
{
	throw new Exception("Cake process Failed on Build Task");
});

Task("Get-Fx-cop-Reports")
 .Does(() =>
 { 
	if (DirectoryExists(FXReportDir))
    {
	 DeleteDirectory(FXReportDir, new DeleteDirectorySettings {
    Recursive = true,
    Force = true
});
	}	 
	var fxCopCount=0;
	var warningsFiles = GetFiles("../cireports/warnings/*.txt");
    foreach(var warningsFile in warningsFiles){
    if(FileExists(waringsFolder+warningsFile.GetFilename())) {
	var fxCopWarning = FileReadLines(waringsFolder+warningsFile.GetFilename() );
    if(fxCopWarning.Length != 0)
    {        
	   fxCopCount+=fxCopWarning.Length;
       
    }
	}
	}
	Information("There are {0} FXCop violations found", fxCopCount);
	if (!DirectoryExists(FXReportDir)) {
		CreateDirectory(FXReportDir);
	}
	FileWriteText(fxCopReport, "FXCop Error(s) : " + fxCopCount);
	if (DirectoryExists(waringsFolder))
    {
	 DeleteDirectory(waringsFolder, new DeleteDirectorySettings {
    Recursive = true,
    Force = true
});
	}
	if (DirectoryExists(errorlogsFolder))
    {
	 DeleteDirectory(errorlogsFolder, new DeleteDirectorySettings {
    Recursive = true,
    Force = true
});	}
}).OnError(exception =>
{
	throw new Exception("Cake process Failed on Build Task");
});
// The default task to run if none is explicitly specified.
Task("Default")
    .IsDependentOn("Build");
	//.IsDependentOn("codeviolation");
 
// Executes the task specified in the target argument.
RunTarget(target);
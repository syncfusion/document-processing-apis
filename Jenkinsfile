node('FileFormat')
{ 
timestamps 
{
  timeout(time: 7200000, unit: 'MILLISECONDS') 
  {
    stage 'Checkout' 
    try
    {	    
	  checkout scm   
	}
 
    catch(Exception e)
    {   
		currentBuild.result = 'FAILURE'
    } 

if(currentBuild.result != 'FAILURE')
{ 
	stage 'Build Source'
	try
	{
	    gitlabCommitStatus("Build")
		{
			bat 'powershell.exe -ExecutionPolicy ByPass -File build/build.ps1 -Script '+env.WORKSPACE+"/build/build.cake -Target Build"
	 	}

    }
	 catch(Exception e) 
    {
		currentBuild.result = 'FAILURE'
    }
}
//if(currentBuild.result != 'FAILURE')
//{
//	stage 'Code violation'
//	try
//	{
//		gitlabCommitStatus("Code violation")
//		{
//			bat 'powershell.exe -ExecutionPolicy ByPass -File build/build.ps1 -Script '+env.WORKSPACE+"/build/build.cake -Target codeviolation"
//		}
//	}
//	catch(Exception e) 
//	{
//		echo "Exception in code violation stage \r\n"+e
//		currentBuild.result = 'FAILURE'
//	}
//}	
	stage 'Delete Workspace'
	
	// Archiving artifacts when the folder was not empty
	
//    def files = findFiles(glob: '**/cireports/**/*.*')      
    
//    if(files.size() > 0) 		
//    { 		
//        archiveArtifacts artifacts: 'cireports/', excludes: null 		
//    }

	    step([$class: 'WsCleanup'])	
		}
}}

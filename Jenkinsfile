pipeline {
  agent none
  stages {
    stage('Build and Test') {
      agent {
        dockerfile {
			    filename "Dockerfile.test"
		    } 
      }
      environment {
        // https://stackoverflow.com/questions/53556623/dotnet-build-permission-denied-in-docker-container-running-jenkins
        HOME = '/tmp'
      } 
      stages {
        stage('Build') {
          steps {
            sh "cd /source"
            sh "ls"
            sh "dotnet build -c Release -warnaserror"
          }
        }
        stage('Test') {
          steps {
			  sh "dotnet test --no-build --nologo -c Release --verbosity d"
          }
        }
      }
    //   post {
    //     success {
    //        stash name: 'artifacts', includes: "node_modules/**/*"
    //     }
    //   }
    }
  }
}
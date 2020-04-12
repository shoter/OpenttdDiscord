pipeline {
  agent none
  parameters {
        booleanParam(defaultValue: true, description: '', name: 'userFlag')
        booleanParam(defaultValue: true, description: '', name: 'userFlag')
        booleanParam(defaultValue: true, description: '', name: 'userFlag')
  }

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
            sh "dotnet build -c Release -warnaserror"
          }
        }
        stage('Test') {
          steps {
			     sh "dotnet test --no-build --nologo -c Release"
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
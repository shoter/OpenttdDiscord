pipeline {
  agent none
  stages {
    stage('Build and Test') {
      agent {
        dockerfile {
			    filename "Dockerfile.test"
		    } 
      }
      stages {
        stage('Build') {
          steps {
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
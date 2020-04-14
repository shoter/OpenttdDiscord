pipeline {
  agent none
  parameters {
        credentials(name: 'DISCORD_TOKEN', description: '', defaultValue: '', credentialType: "Secret text", required: true)
        credentials(name: 'MYSQL_CONN', description: '', defaultValue: '', credentialType: "Secret text", required: true)
        string(name: 'IMAGE_VERSION', defaultValue: 'test', description: '')
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

        stage('Create image') {
        agent any
        stages {
          stage('Build') {
            steps {
              sh "build.sh"
            }
          }
          stage('Test') {
            steps {
            sh "dotnet test --no-build --nologo -c Release"
            }
          }
        }
      }
    }

    
  }
}
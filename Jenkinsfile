pipeline {
  agent none
  stages {
    stage('Build and Test') {
      agent {
        docker 'mcr.microsoft.com/dotnet/core/sdk:3.1'
      }
      stages {
        stage('Build') {
          steps {
			sh "dotnet build"
          }
        }
        stage('Test') {
          steps {
			  sh "dotnet test"
          }
        }
      }
      post {
        success {
          // stash name: 'artifacts', includes: "node_modules/**/*"
        }
      }
    }
  }
}
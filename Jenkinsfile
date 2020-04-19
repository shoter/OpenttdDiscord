pipeline {
  agent none
  environment {
        SSH_REMOTE = credentials("ottd_discord_remote_${env.BRANCH_NAME}")
        IMAGE_VERSION = "${env.BRANCH_NAME}"
        DISCORD_TOKEN = credentials("ottd_discord_token_${env.BRANCH_NAME}")        
        MYSQL_CONN = credentials("ottd_discord_mysql_${env.BRANCH_NAME}")
        CONTAINER_NAME = "openttd_discord_${env.BRANCH_NAME}"
        DOCKER_HUB = credentials("docker_hub")
    }  

  stages {
    stage("Build MySQL image") {
      agent any
      steps {
        sh "/bin/bash ./OpenttdDiscord.Database/SQL/build.sh"
      }
    }
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
            echo env.BRANCH_NAME
            sh "dotnet build -c Release -warnaserror"
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
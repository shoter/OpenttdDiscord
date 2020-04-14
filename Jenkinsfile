pipeline {
  agent none
  parameters {
        credentials(name: 'DISCORD_TOKEN', description: '', defaultValue: "ottd_discord_token_${env.GIT_BRANCH}", credentialType: "Secret text", required: true)
        credentials(name: 'MYSQL_CONN', description: '', defaultValue: "ottd_discord_mysql_${env.GIT_BRANCH}", credentialType: "Secret text", required: true)
        credentials(name: 'SSH_REMOTE', description: '', defaultValue: "ottd_discord_remote_${env.GIT_BRANCH}", credentialType: "Username with password", required: true)
        string(name: 'IMAGE_VERSION', defaultValue: "${env.GIT_BRANCH}", description: '')
  }

  environment {
        SSH_REMOTE = credentials("${params.SSH_REMOTE}")
        IMAGE_VERSION = credentials("${params.IMAGE_VERSION}")
        DISCORD_TOKEN = credentials("${params.DISCORD_TOKEN}")        
        MYSQL_CONN = credentials("${params.MYSQL_CONN}")
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
    }

        stage('Create image') {
        agent any
        stages {
          stage('Build') {
            steps {
              sh "./build.sh"
            }
          }
          stage('Deploy') {
            agent any
            steps {
              script {
                def remote = [:]
                remote.name = 'pir'
                remote.host = 'pir.ja.dom'
                remote.user = SSH_REMOTE_USR
                remote.password = SSH_REMOTE_PSW
                remote.allowAnyHosts = true
                sshCommand remote: remote, command: "docker stop openttd_discord:${IMAGE_VERSION} || true && docker rm openttd_discord:${IMAGE_VERSION} || true"
                sshCommand remote: remote, command: "docker run -d --name=\"openttd_discord:${IMAGE_VERSION}\" \
                                                    -e ottd_discord_token=\"${DISCORD_TOKEN}\" \
                                                    -e MYSQL_CONN=\"${MYSQL_CONN}\" \
                                                    --restart always \
                                                    \"openttd_discord:${IMAGE_VERSION}\""
              }
            }
          }
        }
      }
    }

    
}
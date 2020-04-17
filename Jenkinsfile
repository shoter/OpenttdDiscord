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
    // stage('Build and Test') {
    //   agent {
    //     dockerfile {
		// 	    filename "Dockerfile.test"
		//     } 
    //   }
    //   environment {
    //     // https://stackoverflow.com/questions/53556623/dotnet-build-permission-denied-in-docker-container-running-jenkins
    //     HOME = '/tmp'
    //   } 
    //   stages {
    //     stage('Build') {
    //       steps {
    //         echo env.BRANCH_NAME
    //         sh "dotnet build -c Release -warnaserror"
    //       }
    //     }
    //     stage('Test') {
    //       steps {
		// 	     sh "dotnet test --no-build --nologo -c Release"
    //       }
    //     }
    //   }
    // }

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
                sshCommand remote: remote, command: "docker stop ${CONTAINER_NAME} || true && docker rm ${CONTAINER_NAME} || true"
                sshCommand remote: remote, command: "docker login --username ${DOCKER_HUB_USR} --password ${DOCKER_HUB_PSW}"
                sshCommand remote: remote, command: "docker run -d --name=\"${CONTAINER_NAME}\" \
                                                    -e ottd_discord_token=\"${DISCORD_TOKEN}\" \
                                                    -e MYSQL_CONN=\"${MYSQL_CONN}\" \
                                                    --restart always \
                                                    \"shoter/openttd-discord:${IMAGE_VERSION}\""
              }
            }
          }
        }
      }
    }

    
}
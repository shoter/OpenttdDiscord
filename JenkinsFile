pipeline {
  agent any
  stages {
  stage("Clean") {
		steps {
			script {
			sh "dotnet clean"
			}
		}
  }
  stage("Build") {
		steps {
			script {
			sh "dotnet build -c Release -warnaserror"
			}
		}
  }
  stage("Test") {
		steps {
			script {
			sh "dotnet test --no-build --nologo -c Release --verbosity d"
			}
		}
  }
}
}

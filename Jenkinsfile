pipeline {
  agent any
  stages {
     stage('Verify Branch') {
       steps {
         echo "$GIT_BRANCH"
	}
    }
      stage('Docker Build') {
        steps {
         powershell 'docker-compose build'
         powershell 'docker images -a'
              }
       }
    }
}
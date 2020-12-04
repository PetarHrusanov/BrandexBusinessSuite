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
         'docker-compose build'     
         'docker images -a'
              }
       }
    }
}
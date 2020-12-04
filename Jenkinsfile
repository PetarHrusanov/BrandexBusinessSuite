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
         sh 'docker build ./docker-compose'   
         sh 'docker images -a'
              }
       }
    }
}
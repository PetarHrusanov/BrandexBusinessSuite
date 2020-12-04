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
          sh "./docker-compose up --build -d"   
          sh 'docker images -a'
              }
       }
    }
}
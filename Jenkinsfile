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
          sh('cd relativePathToFolder && chmod +x docker-compose.yml build')
              }
       }
    }
}
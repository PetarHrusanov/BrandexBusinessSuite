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
          node {
           pwsh(script: 'docker-compose build')
           pwsh(script: 'docker images -a')
                }
              }
       }
    }
}
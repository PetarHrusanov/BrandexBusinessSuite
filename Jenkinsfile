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
         sh "/Users/Petar/.config/powershell -Command \"Get-Host | Select-Object Version\""
         pwsh(script: 'docker-compose build')
         pwsh(script: 'docker images -a')
              }
       }
    }
}
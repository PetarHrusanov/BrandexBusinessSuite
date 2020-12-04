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
         sh "/usr/bin:/bin:/usr/sbin:/sbin:/usr/local/bin/pwsh -Command \"Get-Host | Select-Object Version\""
         pwsh(script: 'docker-compose build')
         pwsh(script: 'docker images -a')
              }
       }
    }
}
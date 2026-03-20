pipeline {
    agent any

    environment {
        // Configure these in Jenkins or as pipeline parameters
        DOCKER_REGISTRY = 'git.wrigglyt.xyz'
        DOCKER_IMAGE = 'chloe/seasoned'  // e.g., username/repo for Docker Hub
        DEPLOY_HOST = '10.0.11.3'
        DEPLOY_USER = 'chloe'
        DEPLOY_PATH = '/opt/seasoned'
        GIT_REPO_URL = 'https://git.wrigglyt.xyz/chloe/Seasoned.git'
    }

    options {
        buildDiscarder(logRotator(numToKeepStr: '10'))
        timeout(time: 30, unit: 'MINUTES')
    }

    stages {
        stage('Build Docker Image') {
            steps {
                script {
                    env.IMAGE_TAG = env.BRANCH_NAME == 'main' ? 'latest' : "${env.BUILD_NUMBER}-${env.GIT_COMMIT.take(7)}"
                    env.FULL_IMAGE = "${env.DOCKER_REGISTRY}/${env.DOCKER_IMAGE}:${env.IMAGE_TAG}"
                }
                echo "Building image: ${env.FULL_IMAGE}"
                sh """
                    docker build -t ${env.FULL_IMAGE} .
                    docker tag ${env.FULL_IMAGE} ${env.DOCKER_REGISTRY}/${env.DOCKER_IMAGE}:latest
                """               
            }
        }

        stage('Push to Registry') {
            steps {
                withCredentials([usernamePassword(
                    credentialsId: 'c-gitea',
                    usernameVariable: 'DOCKER_USER',
                    passwordVariable: 'DOCKER_PASS'
                )]) {
                    sh """
                        echo \$DOCKER_PASS | docker login -u \$DOCKER_USER --password-stdin ${env.DOCKER_REGISTRY}
                        docker push ${env.FULL_IMAGE}
                        docker push ${env.DOCKER_REGISTRY}/${env.DOCKER_IMAGE}:latest
                    """
                }
            }
        }

        stage('Deploy via SSH') {
            steps {
                script {
                    env.DEPLOY_BRANCH = env.BRANCH_NAME ?: 'main'
                }
                sshagent(credentials: ['mini2']) {
                    sh """
                        ssh -o StrictHostKeyChecking=no ${env.DEPLOY_USER}@${env.DEPLOY_HOST} << 'DEPLOY_EOF'
                            set -e
                            cd ${env.DEPLOY_PATH} || mkdir -p ${env.DEPLOY_PATH} && cd ${env.DEPLOY_PATH}
                            
                            if [ -f .env ]; then cp .env /tmp/.seasoned_env_bak; fi

                            git fetch origin
                            git reset --hard origin/${env.DEPLOY_BRANCH}

                            if [ -f /tmp/.seasoned_env_bak ]; then mv /tmp/.seasoned_env_bak .env; fi

                            export IMAGE_TAG=${env.IMAGE_TAG}
                            export DOCKER_REGISTRY=${env.DOCKER_REGISTRY}
                            export DOCKER_IMAGE=${env.DOCKER_IMAGE}

                            docker compose --env-file .env pull
                            docker compose --env-file .env up -d --force-recreate
DEPLOY_EOF
                    """
                }
            }
        }
    }

    post {
        success {
            echo "Deployment successful! Image: ${env.FULL_IMAGE}"
        }
        failure {
            echo "Deployment failed!"
        }
    }
}

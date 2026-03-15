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
                            
                            # Clone or pull the repo (contains docker-compose.yml)
                            if [ -d .git ]; then
                                git fetch origin
                                git reset --hard origin/${env.DEPLOY_BRANCH}
                            else
                                git clone ${env.GIT_REPO_URL} .
                            fi
                            
                            # Set the image tag for this deployment
                            export IMAGE_TAG=${env.IMAGE_TAG}
                            export DOCKER_REGISTRY=${env.DOCKER_REGISTRY}
                            export DOCKER_IMAGE=${env.DOCKER_IMAGE}
                            
                            # Pull latest image and deploy
                            docker compose pull
                            docker compose up -d
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

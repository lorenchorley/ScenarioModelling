param(
    [string]$Version = "1.0.0",
    [switch]$Debug
)

$ImageName = "lchorley/scenariomodelling:$Version"
$TemporaryDockerImageName = "image-from-local-docker-repo.temp.tar"

$dockerBuildArgs = @("..", "-f", "dockerfile-blazor", "-t", $ImageName, "--build-arg", "BUILD_VERSION=$Version")
if ($Debug) {
    $dockerBuildArgs += @("--progress=plain", "--no-cache")
}

docker build @dockerBuildArgs

docker save $ImageName -o $TemporaryDockerImageName
# minikube image rm $ImageName
minikube image load $TemporaryDockerImageName
rm $TemporaryDockerImageName

@echo off
set "UnityPath=D:\Unity\Editor\2022.3.62f3\Editor\Unity.exe"
set "ProjectPath=%~dp0"

echo ===== 开始打包 HarvestFarm (Windows) =====
"%UnityPath%" -projectPath "%ProjectPath%" -quit -batchmode -executeMethod JenkinsBuild.BuildWindows -logFile "%ProjectPath%build.log"

echo Unity 退出码: %errorlevel%
if not "%errorlevel%"=="0" goto fail

echo ===== 打包成功 =====
exit /b 0

:fail
echo ===== 打包失败，请查看 build.log =====
exit /b 1

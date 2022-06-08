ECHO OFF
CHCP 65001
::Variáveis
SET filesDir=D:\projetos\github\Unimake.DFe\source\Unimake.DFe\Compilacao\INTEROP_Release\
SET istool="D:\Program Files (x86)\Inno Setup 6\ISCC.exe"

::Prepara
DEL /S /Q %cd%\err
RD /S /Q %filesDir%
CLS

@ECHO ----------------------------------------------------------------------------------
@ECHO Compilando Unimake.DFe
@ECHO ----------------------------------------------------------------------------------

dotnet build D:\projetos\github\Unimake.DFe\source\Unimake.DFe.sln --configuration INTEROP_Release --force

@ECHO:
@ECHO:
@ECHO:
@ECHO:
@ECHO:
@ECHO ----------------------------------------------------------------------------------
@ECHO Limpando diretório de release
@ECHO ----------------------------------------------------------------------------------

::Apaga os arquivos desnecessários
DEL /S /Q %filesDir%\*.xml
DEL /S /Q %filesDir%\*.pdb
DEL /S /Q %filesDir%\*.json
DEL /S /Q %filesDir%\App.config
DEL /S /Q %filesDir%\TesteDLL_Unimake.Business.DFe.exe
DEL /S /Q %filesDir%\Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll
DEL /S /Q %filesDir%\TesteDLL_Unimake.Business.DFe.exe.config
:: Esta dll tem que pegar da pasta do VB6
DEL /S /Q %filesDir%\System.Security.Cryptography.Xml.dll 

::Apaga os arquivos desnecessários
DEL /S /Q %filesDir%\net462\*.xml
DEL /S /Q %filesDir%\net462\*.pdb
DEL /S /Q %filesDir%\net462\*.json
DEL /S /Q %filesDir%\net462\App.config

::Apaga os arquivos desnecessários
DEL /S /Q %filesDir%\net472\*.xml
DEL /S /Q %filesDir%\net472\*.pdb
DEL /S /Q %filesDir%\net472\*.json
DEL /S /Q %filesDir%\net472\App.config
DEL /S /Q %filesDir%\net472\Unimake.Business.DFe.dll

::Copia a Unimake.Utils
copy C:\projetos\github\Unimake.DFe\source\Unimake.DFe.Test\bin\Release\netcoreapp3.1\Unimake.Utils.dll %filesDir%\netstandard2.0
copy C:\projetos\github\Unimake.DFe\source\Unimake.DFe.Test\bin\Release\netcoreapp3.1\Unimake.Cryptography.dll %filesDir%\netstandard2.0
copy C:\projetos\github\Unimake.DFe\source\Unimake.DFe.Test\bin\Release\netcoreapp3.1\Unimake.Extensions.dll %filesDir%\netstandard2.0

::Ações
@ECHO ----------------------------------------------------------------------------------
@ECHO Assinando executáveis e dlls
@ECHO ----------------------------------------------------------------------------------
FORFILES /p %filesDir% /s /m unimake.* /c "cmd /c %CD%\sign.bat @path %cd%"

@ECHO ----------------------------------------------------------------------------------
@ECHO Compilando script
@ECHO ----------------------------------------------------------------------------------

CALL %istool% Unimake.DFe.iss

@ECHO:
@ECHO ----------------------------------------------------------------------------------
@ECHO Verifique as mensagens de erro. Pressione CTRL+C para terminar a compilação ou ...
@ECHO ----------------------------------------------------------------------------------

@ECHO:
@ECHO:
@ECHO:
@ECHO:
@ECHO:
@ECHO:
@ECHO ----------------------------------------------------------------------------------
@ECHO Assinando o instalador
@ECHO ----------------------------------------------------------------------------------
CALL sign %cd%\output\Install_Unimake.DFe.exe %cd%
EXPLORER %cd%\output

:ok
exit /B 0
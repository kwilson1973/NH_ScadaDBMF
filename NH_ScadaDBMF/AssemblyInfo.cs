using System.Reflection;
using System.Runtime.CompilerServices;

//
// 一般的組件資訊是由下列這組屬性所控制。
// 變更這些屬性的值即可修改組件的相關資訊。
//
[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// 組件的版本資訊由下列四個值所組成:
//
//      主要版本
//      次要版本 
//      組建編號
//      修訂
//
// 您可以自行指定所有的值，也可以依照以下的方式使用 '*' 將修訂和組建編號指定為預設值:

[assembly: AssemblyVersion("1.1.20.*")]
[assembly: AssemblyFileVersion("2017.09.28.1")]

//
// 若要簽名組件，您必須指定要使用的金鑰。
// 如需組件簽名的詳細資訊，請參閱 Microsoft .NET Framework 文件。
//
// 請使用下列屬性來控制要使用哪個金鑰來簽名。
//
// 注意: 
//   (*) 如果沒有指定金鑰，組件將不會被簽名。
//   (*) KeyName 是指已安裝在電腦上密碼編譯服務提供者 (CSP) 中的金鑰。
//       KeyFile 是指含有金鑰的檔案。
//   (*) 如果同時指定了 KeyFile 和 KeyName 的值，將會發生下列的處理:
//       (1) 如果在 CSP 中可以找到 KeyName，就會使用這一個金鑰。
//       (2) 如果 KeyName 不存在而 KeyFile 卻存在，就會將 KeyFile 中的金鑰安裝到 CSP 中
//           並且使用它。
//   (*) 若要建立 KeyFile，您可以使用 sn.exe (強式名稱) 公用程式。
//       在指定 KeyFile 時，KeyFile 的位置應該相對於專案輸出目錄，亦即
//        %Project Directory%\obj\<configuration>。
//       例如，如果您的 KeyFile 是位於專案目錄中，您應該將 AssemblyKeyFile 屬性指定為
//        [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) 延遲簽名是一個進階選項 - 如需詳細資訊，請參閱 Microsoft .NET Framework 文件。
//
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]


namespace Editor.SampleEditorAppTemplate;


/// <summary>
/// container for readonly editor app configuration options.
/// </summary>
public static class EditorAppInfo 
{
	/// <summary>
	/// Name of the editor app.
	/// </summary>
	public static string Name => "Sample Editor App";

	/// <summary>
	/// Freindly name of the editor app usually the same as the AppName but without spaces.
	/// </summary>
	public static string NameFriendly => "sampleeditorapp";

	/// <summary>
	/// Library folder name of the project that this editor app resides in.
	/// </summary>
	public static string LibraryFolderName => "SampleEditorApp";

	/// <summary>
	/// The current version of the editor app.
	/// </summary>
	public static string Version => "1.0";

	/// <summary>
	/// The icon of the editor app.
	/// </summary>
	public static string AppIcon => $"{ToolsFolderPath}/images/sampletool/appicon.png";

	/// <summary>
	/// File extention of the asset that your editor app uses if any.
	/// </summary>
	public static string AssetFileExtention => ".txt";

	/// <summary>
	/// Title of the windows file picker.
	/// </summary>
	public static string AssetFilePickerTitle => $"{Name} File";

	/// <summary>
	/// Name filter of the windows file picker.
	/// </summary>
	public static string AssetFilePickerNameFilter => "Example File";

	/// <summary>
	/// Tools folder path. usually contains an images folder with images for the editor app itself to use.
	/// </summary>
	public static string ToolsFolderPath => Utils.Paths.ChooseExistingPath( Utils.Paths.GetLibaryAbsolutePath( $"{LibraryFolderName}/tools" ), Utils.Paths.GetLibaryAbsolutePath( $"{LibraryOrgIdent}.{LibraryPackageIdent}/tools" ) );

	/// <summary>
	/// Ident of the the Library that this editor app is in. Doing it this way so that there is no need to change any ident reference in code.
	/// </summary>
	public static string LibraryIdent => Utils.ProjectHelpers.GetIdentFromExecutingAssemblyName();

	/// <summary>
	/// Org Ident of the the Library that this editor app is in.
	/// </summary>
	public static string LibraryOrgIdent => Utils.ProjectHelpers.GetLibraryOrgIdent( LibraryIdent );

	/// <summary>
	/// Package Ident of the the Library that this editor app is in.
	/// </summary>
	public static string LibraryPackageIdent => Utils.ProjectHelpers.GetLibraryPackageIdent( LibraryIdent );
}

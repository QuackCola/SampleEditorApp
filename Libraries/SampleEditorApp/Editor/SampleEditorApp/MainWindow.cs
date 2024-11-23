using System;
using System.Text.RegularExpressions;

namespace Editor.SampleEditorAppTemplate;

//[EditorForAssetType( "" )]
[EditorApp( "Sample Editor App", "category", "Sample editor app" )]
public class MainWindow : DockWindow, IAssetEditor
{
	//private Asset _asset;
	private bool _dirty = false;
	private Menu _recentFilesMenu;
	private readonly List<string> _recentFiles = new();
	private string _defaultDockState;
	public bool CanOpenMultipleAssets => true;

	public MainWindow()
	{
		DeleteOnClose = true;
		
		Title = EditorAppInfo.Name;
		SetWindowIcon( Pixmap.FromFile( EditorAppInfo.AppIcon ) );
		Size = new Vector2( 1700, 1050 );

		_recentFiles = FileSystem.Temporary.ReadJsonOrDefault( $"{EditorAppInfo.NameFriendly}_recentfiles.json", _recentFiles )
			.Where( x => System.IO.File.Exists( x ) ).ToList();

		CreateUI();
		Show();
		
		CreateNew();
	}

	private void OpenAboutDialog()
	{
		var labeltext = $@"
			<h2> {EditorAppInfo.Name} </h2> 
			<p> Version: {EditorAppInfo.Version} <p>
		";

		var about = new AboutDialog(
			$"About {EditorAppInfo.Name}",
			labeltext,
			EditorAppInfo.AppIcon
		);

		about.Show();
	}

	public void AssetOpen( Asset asset )
	{
		if ( asset == null || string.IsNullOrWhiteSpace( asset.AbsolutePath ) )
			return;

		Open( asset.AbsolutePath );
	}

	[EditorEvent.Frame]
	protected void Frame()
	{
	}

	[Shortcut( "editor.undo", "CTRL+Z" )]
	private void Undo()
	{
		Log.Info("Undo called!");
	}

	[Shortcut( "editor.redo", "CTRL+Y" )]
	private void Redo()
	{
		Log.Info( "Redo called!" );
	}

	private void SetUndoLevel( int level )
	{
		Log.Info( "SetUndoLevel called!" );
	}

	[Shortcut( "editor.cut", "CTRL+X" )]
	private void CutSelection()
	{
		Log.Info( "CutSelection called!" );
	}

	[Shortcut( "editor.copy", "CTRL+C" )]
	private void CopySelection()
	{
		Log.Info( "CopySelection called!" );
	}

	[Shortcut( "editor.paste", "CTRL+V" )]
	private void PasteSelection()
	{
		Log.Info( "PasteSelection called!" );
	}

	[Shortcut( "editor.select-all", "CTRL+A" )]
	private void SelectAll()
	{
		Log.Info( "SelectAll called!" );
	}
	
	public void BuildMenuBar()
	{
		var file = MenuBar.AddMenu( "File" );
		file.AddOption( "New", "common/new.png", New, "editor.new" ).StatusTip = "New File";
		file.AddOption( "Open", "common/open.png", Open, "editor.open" ).StatusTip = "Open File";
		file.AddOption( "Save", "common/save.png", Save, "editor.save" ).StatusTip = "Save File";
		file.AddOption( "Save As...", "common/save.png", SaveAs, "editor.save-as" ).StatusTip = "Save File As...";

		file.AddSeparator();

		_recentFilesMenu = file.AddMenu( "Recent Files" );

		file.AddSeparator();

		file.AddOption( "Quit", null, Quit, "editor.quit" ).StatusTip = "Quit";

		var edit = MenuBar.AddMenu( "Edit" );

		edit.AddSeparator();
		edit.AddOption( "Cut", "common/cut.png", CutSelection, "editor.cut" );
		edit.AddOption( "Copy", "common/copy.png", CopySelection, "editor.copy" );
		edit.AddOption( "Paste", "common/paste.png", PasteSelection, "editor.paste" );
		edit.AddOption( "Select All", "select_all", SelectAll, "editor.select-all" );

		var view = MenuBar.AddMenu( "View" );

		view.AboutToShow += () => OnViewMenu( view );

		var help = MenuBar.AddMenu( "Help" );

		help.AddOption( $"About {EditorAppInfo.Name}", "helpsystem/help_editor_app_icon.png", OpenAboutDialog, null );


		RefreshRecentFiles();
	}

	void RefreshRecentFiles()
	{
		_recentFilesMenu.Enabled = _recentFiles.Count > 0;

		_recentFilesMenu.Clear();

		_recentFilesMenu.AddOption( "Clear recent files", null, ClearRecentFiles ).StatusTip = "Clear recent files";

		_recentFilesMenu.AddSeparator();

		const int maxFilesToDisplay = 10;
		int fileCount = 0;

		for ( int i = _recentFiles.Count - 1; i >= 0; i-- )
		{
			if ( fileCount >= maxFilesToDisplay )
				break;

			var filePath = _recentFiles[i];

			_recentFilesMenu.AddOption( $"{++fileCount} - {filePath}", null, () => PromptSave( () => Open( filePath ) ) ).StatusTip = $"Open {filePath}";
		}
	}

	private void OnViewMenu( Menu view )
	{
		view.Clear();
		view.AddOption( "Restore To Default", "settings_backup_restore", RestoreDefaultDockLayout );
		view.AddSeparator();

		foreach ( var dock in DockManager.DockTypes )
		{
			var o = view.AddOption( dock.Title, dock.Icon );
			o.Checkable = true;
			o.Checked = DockManager.IsDockOpen( dock.Title );
			o.Toggled += ( b ) => DockManager.SetDockState( dock.Title, b );
		}
	}

	private void ClearRecentFiles()
	{
		if ( _recentFiles.Count == 0 )
			return;

		_recentFiles.Clear();

		RefreshRecentFiles();

		SaveRecentFiles();
	}

	private void AddToRecentFiles( string filePath )
	{
		filePath = filePath.ToLower();

		// If file is already recent, remove it so it'll become the most recent
		if ( _recentFiles.Contains( filePath ) )
		{
			_recentFiles.RemoveAll( x => x == filePath );
		}

		_recentFiles.Add( filePath );

		RefreshRecentFiles();
		SaveRecentFiles();
	}

	private void SaveRecentFiles()
	{
		FileSystem.Temporary.WriteJson( $"{EditorAppInfo.NameFriendly}_recentfiles.json", _recentFiles );
	}

	private void PromptSave( Action action )
	{
		if ( !_dirty )
		{
			action?.Invoke();
			return;
		}

		var confirm = new PopupWindow(
			"Save Current File", "The open file has unsaved changes. Would you like to save now?", "Cancel",
			new Dictionary<string, Action>()
			{
				{ "No", () => action?.Invoke() },
				{ "Yes", () => { if ( SaveInternal( false ) ) action?.Invoke(); } }
			}
		);

		confirm.Show();
	}

	[Shortcut( "editor.new", "CTRL+N" )]
	public void New()
	{
		PromptSave( CreateNew );
	}

	public void CreateNew()
	{
		_dirty = false;
	}

	public void Open()
	{

		var fd = new FileDialog( null )
		{
			Title = $"Open {EditorAppInfo.AssetFilePickerTitle}",
			DefaultSuffix = EditorAppInfo.AssetFileExtention
		};

		fd.SetNameFilter( $"{EditorAppInfo.AssetFilePickerNameFilter} ({EditorAppInfo.AssetFileExtention})" );

		if ( !fd.Execute() )
			return;

		PromptSave( () => Open( fd.SelectedFile ) );
	}

	public void Open( string path )
	{
		var asset = AssetSystem.FindByPath( path );
		if ( asset == null )
			return;

		//if ( asset == _asset )
		//{
		//	Log.Warning( $"{asset.RelativePath} is already open" );
		//	return;
		//}
		
		//_assetRelativePath = asset.RelativePath;
		//_asset = asset;

		AddToRecentFiles( path );
	}


	[Shortcut( "editor.save-as", "CTRL+SHIFT+S" )]
	public void SaveAs()
	{
		SaveInternal( true );
	}

	[Shortcut( "editor.save", "CTRL+S" )]
	public void Save()
	{
		SaveInternal( false );
	}

	private bool SaveInternal( bool saveAs )
	{
		return true;
	}

	private static string GetSavePath()
	{
		return "";
	}

	public void CreateUI()
	{
		BuildMenuBar();

		DockManager.RegisterDockType( "Console", "text_snippet", null, false );

		// Yuck, console is internal but i want it, what is the correct way?
		var console = EditorTypeLibrary.Create( "ConsoleWidget", typeof( Widget ), new[] { this } ) as Widget;
		DockManager.AddDock( null, console, DockArea.Inside, DockManager.DockProperty.HideOnClose );

		DockManager.RaiseDock( "console" );
		DockManager.Update();

		_defaultDockState = DockManager.State;

		if ( StateCookie != EditorAppInfo.StateCookieName )
		{
			StateCookie = EditorAppInfo.StateCookieName;
		}
		else
		{
			RestoreFromStateCookie();
		}
	}

	private void SetDirty()
	{ 
	
	}

	private void OnPropertyUpdated()
	{
		Log.Info("property updated!");
		SetDirty();
	}

	protected override void RestoreDefaultDockLayout()
	{
		DockManager.State = _defaultDockState;

		SaveToStateCookie();
	}

	protected override bool OnClose()
	{
		if ( !_dirty )
		{
			return true;
		}

		var confirm = new PopupWindow(
			"Save Current File", "The open file has unsaved changes. Would you like to save now?", "Cancel",
			new Dictionary<string, Action>()
			{
				{ "No", () => { _dirty = false; Close(); } },
				{ "Yes", () => { if ( SaveInternal( false ) ) Close(); } }
			}
		);

		confirm.Show();

		return false;
	}

	[Shortcut( "editor.quit", "CTRL+Q" )]
	void Quit()
	{
		Close();
	}

	[EditorEvent.Hotload]
	public void OnHotload()
	{
		DockManager.Clear();
		MenuBar.Clear();

		CreateUI();
	}

	void IAssetEditor.SelectMember( string memberName )
	{
		throw new NotImplementedException();
	}
}

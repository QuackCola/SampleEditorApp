using System.Text.RegularExpressions;

namespace Editor.SampleEditorAppTemplate;

public static class Utils
{

	public static class ProjectHelpers
	{
		/// <summary>
		/// Uses Regex pattern matching to fetch the Ident from the project's executing assembly name.
		/// </summary>
		public static string GetIdentFromExecutingAssemblyName()
		{
			string executingAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;

			string pattern = @"^[^.]+\.[^.]+\.([^.]+)\.";

			Match match = Regex.Match( executingAssemblyName, pattern );

			var result = "";

			if ( match.Success )
			{
				result = match.Groups[1].Value;
			}

			return result;
		}


		/// <summary>
		/// Gets the org ident of the matching library package ident.
		/// </summary>
		public static string GetLibraryOrgIdent( string ident )
		{
			var libraryOrg = "";

			foreach ( var library in LibrarySystem.All )
			{
				if ( library.Project.Package.Ident == ident )
				{
					libraryOrg = library.Project.Package.Org.Ident;
				}
			}

			return libraryOrg;
		}

		/// <summary>
		/// Gets the matching library package ident.
		/// </summary>
		public static string GetLibraryPackageIdent( string ident )
		{
			var libraryIdent = "";

			foreach ( var library in LibrarySystem.All )
			{
				if ( library.Project.Package.Ident == ident )
				{
					libraryIdent = library.Project.Package.Ident;
				}
			}
			return libraryIdent;
		}
	}

	public static class Paths
	{
		/// <summary>
		/// Absolute path to the location of the .sbproj file of the project.
		/// </summary>
		public static string GetProjectAbsolutePath()
		{
			return Project.Current.GetRootPath().Replace( '\\', '/' );
		}

		/// <summary>
		/// Relative path to the location of the .sbproj file of the project from the provided path.
		/// </summary>
		public static string GetProjectRelativePath( string path )
		{
			return Path.GetRelativePath( GetProjectAbsolutePath(), path ).Replace( '\\', '/' );
		}

		/// <summary>
		/// Absolute path to a file or directory thats within a mounted library project.
		/// </summary>
		public static string GetLibaryAbsolutePath( string path )
		{
			return FileSystem.Libraries.GetFullPath( path ).Replace( '\\', '/' );
		}

		public static string ChooseExistingPath( string path1, string path2 )
		{
			if ( Directory.Exists( path1 ) )
			{
				return path1;
			}
			else if ( Directory.Exists( path2 ) )
			{
				return path2;
			}
			else if ( Directory.Exists( path1 ) || Directory.Exists( path2 ) )
			{
				Log.Error( $"Both path 1 & path 2 exist!" );
				return null;
			}
			else
			{
				return null; // Neither path exists
			}
		}

	}
}

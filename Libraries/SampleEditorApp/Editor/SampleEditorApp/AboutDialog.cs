namespace Editor.SampleEditorAppTemplate;

public class AboutDialog : Dialog
{
	private string WindowIconAbsolutePath;
	Layout OkButtonLayout;

	/// <summary>
	/// Basic about dialog.
	/// </summary>
	public AboutDialog( string dialogTitle, string labelText, string windowIcon , string buttonText = "OK" ) 
	{
		
		WindowIconAbsolutePath = windowIcon;

		Window.WindowTitle = dialogTitle;
		Window.SetWindowIcon( Pixmap.FromFile( WindowIconAbsolutePath ) );
		Window.SetModal( true, true );

		Size = new Vector2( 310, 176 );
		FixedWidth = Size.x;
		FixedHeight = Size.y;

		Layout = Layout.Column();
		Layout.Margin = 16;
		Layout.Spacing = 16;

		OkButtonLayout = Layout.Row();

		var Label = new Label( this );
		Label.Text = labelText;

		Label.AdjustSize();
		Label.Alignment = TextFlag.Right;
		Layout.Add( Label );

		var okButton = new Button( this );
		okButton.Text = buttonText;
		okButton.MouseLeftPress += () => Close();
		okButton.AdjustSize();

		OkButtonLayout.AddStretchCell(0);
		OkButtonLayout.Add( okButton );
		OkButtonLayout.AddStretchCell(0);

		Layout.Add( OkButtonLayout );

		Window.AdjustSize();
		Window.Size += 8; // HACK: Adjust for weird padding between Window and this, visible via the debugger
	}

	protected override void OnPaint()
	{
		base.OnPaint();

		Paint.ClearPen();
		Paint.SetBrush( Theme.WidgetBackground );
		Paint.DrawRect( LocalRect, 1.0f );
		Paint.Antialiasing = true;

		var icon = LocalRect.Shrink( 48, 48 ); ;
		icon.Position = new Vector2( 2, 2 );
		icon.Width = icon.Height;
		
		Paint.SetPen( Theme.White );
		Paint.Draw( icon, $"{WindowIconAbsolutePath}" );

		icon.Left = icon.Right + 8;
		icon.Position = new Vector2( 128, 8 );
	}
}

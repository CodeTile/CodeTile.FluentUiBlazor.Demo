using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace CodeTile.FluentUiBlazor.Demo.Components.Layout;

public partial class MainLayout
{
	[Inject]
	public required IThemeService ThemeService { get; set; }

	private bool DarkTheme { get; set; }

	public async Task SwitchThemeAsync()
	{
		DarkTheme = !DarkTheme;
		await ThemeService.SetThemeAsync(DarkTheme ? ThemeMode.Dark : ThemeMode.Light);

		// Or simply toggle the theme
		// await ThemeService.SwitchThemeAsync();
	}
}

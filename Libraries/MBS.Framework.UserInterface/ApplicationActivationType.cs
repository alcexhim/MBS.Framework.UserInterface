//
//  ActivationType.cs
//
//  Author:
//       Michael Becker <alcexhim@gmail.com>
//
//  Copyright (c) 2022 Mike Becker's Software
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
namespace MBS.Framework.UserInterface
{
	/// <summary>
	/// Specifies the type of activation when the application is started
	/// from layout on a layout-aware operating system.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Values are mutually exclusive and cannot be combined. Each one
	/// relates to a different type of activation, and an app instance can
	/// be activated in only one way at a time.
	/// </para>
	/// <para>
	/// The activation types are based on those supported by Windows 11,
	/// however the enum values are operating system agnostic. Translation
	/// (e.g. by an Engine) must be done before calls to operating system
	/// specific methods are made.
	/// </para>
	/// <para>
	/// On non-layout-aware operating systems, such as Linux, a wrapper
	/// executable (bootstrapper) must be used to invoke the app activation
	/// with the desired <see cref="ApplicationActivationType" />.
	/// </para>
	/// </remarks>
	public enum ApplicationActivationType
	{
		/// <summary>
		/// The activation type was not specified.
		/// </summary>
		Unspecified = -1,

		/// <summary>
		/// The user wants to manage appointments that are provided by the
		/// app.
		/// </summary>
		AppointmentsProvider,
		/// <summary>
		/// The app was activated as a barcode scanner provider.
		/// </summary>
		BarcodeScannerProvider,
		/// <summary>
		/// The user wants to save a file for which the app provides content
		/// management.
		/// </summary>
		CachedFileUpdater,
		/// <summary>
		/// The app captures photos or video from an attached camera.
		/// </summary>
		CameraSettings,
		/// <summary>
		/// The app was launched from the command line.
		/// </summary>
		CommandLineLaunch,
		/// <summary>
		/// Reserved for system use. Introduced in Windows 10, version 1507
		/// (10.0.10240).
		/// </summary>
		ComponentUI,
		/// <summary>
		/// The user wants to handle calls or messages for the phone number
		/// of a contact that is provided by the app.
		/// </summary>
		Contact,
		/// <summary>
		/// The app was launched from the My People UI. Note: introduced in
		/// Windows 10, version 1703 (10.0.15063), but not used. Now used
		/// starting with Windows 10, version 1709 (10.0.16299).
		/// </summary>
		ContactPanel,
		/// <summary>
		/// The user wants to pick contacts.
		/// </summary>
		ContactPicker,
		/// <summary>
		/// The app handles AutoPlay.
		/// </summary>
		Device,
		/// <summary>
		/// This app was activated as a result of pairing a device.
		/// </summary>
		DevicePairing,
		/// <summary>
		/// This app was launched by another app on a different device by
		/// using the DIAL protocol.Introduced in Windows 10, version 1507
		/// (10.0.10240).
		/// </summary>
		DialReceiver,
		/// <summary>
		/// An app launched a file whose file type this app is registered to
		/// handle.
		/// </summary>
		File,
		/// <summary>
		/// The user wants to pick files that are provided by the app.
		/// </summary>
		FileOpenPicker,
		/// <summary>
		/// Reserved for system use. Introduced in Windows 10, version 1607
		/// (10.0.14393).
		/// </summary>
		FilePickerExperience,
		/// <summary>
		/// The user wants to save a file and selected the app as the
		/// location.
		/// </summary>
		FileSavePicker,
		/// <summary>
		/// The app was activated because it was launched by the OS due to a
		/// game's request for Xbox-specific UI. Introduced in Windows 10,
		/// version 1703 (10.0.15063).
		/// </summary>
		GameUIProvider,
		/// <summary>
		/// The user launched the app or tapped a content tile.
		/// </summary>
		Launch,
		/// <summary>
		/// The app was activated as the lock screen. Introduced in Windows
		/// 10, version 1507 (10.0.10240).
		/// </summary>
		LockScreen,
		/// <summary>
		/// Windows Store only. The app launches a call from the lock screen.
		/// If the user wants to accept the call, the app displays its call
		/// UI directly on the lock screen without requiring the user to
		/// unlock. A lock-screen call is a special type of launch
		/// activation.
		/// </summary>
		LockScreenCall,
		/// <summary>
		/// Reserved for system use. Introduced in Windows 10, version 1703
		/// (10.0.15063).
		/// </summary>
		LockScreenComponent,
		/// <summary>
		/// The app was activated in response to a phone call.
		/// </summary>
		PhoneCallActivation,
		/// <summary>
		/// Windows Phone only. The app was activated after the completion of
		/// a picker.
		/// </summary>
		PickerReturned,
		/// <summary>
		/// Windows Phone only. The app was activated after the app was
		/// suspended for a file picker operation.
		/// </summary>
		PickFileContinuation,
		/// <summary>
		/// Windows Phone only. The app was activated after the app was
		/// suspended for a folder picker operation.
		/// </summary>
		PickFolderContinuation,
		/// <summary>
		/// Windows Phone only. The app was activated after the app was
		/// suspended for a file save picker operation.
		/// </summary>
		PickSaveFileContinuation,
		/// <summary>
		/// This app was launched by another app to provide a customized
		/// printing experience for a 3D printer. Introduced in Windows 10,
		/// version 1507 (10.0.10240).
		/// </summary>
		Print3DWorkflow,
		/// <summary>
		/// The app was activated as a print workflow job UI extension.
		/// </summary>
		PrintSupportJobUI,
		/// <summary>
		/// The app was activated as a print support settings UI extension.
		/// </summary>
		PrintSupportSettingsUI,
		/// <summary>
		/// The app handles print tasks.
		/// </summary>
		PrintTaskSettings,
		/// <summary>
		/// The app was activated because the user is printing to a printer
		/// that has a Print Workflow Application associated with it which
		/// has requested user input.
		/// </summary>
		PrintWorkflowForegroundTask,
		/// <summary>
		/// An app launched a URI whose scheme name this app is registered to
		/// handle.
		/// </summary>
		Protocol,
		/// <summary>
		/// The app was launched by another app with the expectation that it
		/// will return a result back to the caller. Introduced in Windows
		/// 10, version 1507 (10.0.10240).
		/// </summary>
		ProtocolForResults,
		/// <summary>
		/// Windows Store only. The user launched the restricted app.
		/// </summary>
		RestrictedLaunch,
		/// <summary>
		/// The user wants to search with the app.
		/// </summary>
		Search,
		/// <summary>
		/// The app is activated as a target for share operations.
		/// </summary>
		ShareTarget,
		/// <summary>
		/// The app was activated because the app is specified to launch at
		/// system startup or user log-in. Introduced in Windows 10, version
		/// 1703 (10.0.15063).
		/// </summary>
		StartupTask,
		/// <summary>
		/// The app was activated when a user tapped on the body of a toast
		/// notification or performed an action inside a toast notification.
		/// Introduced in Windows 10, version 1507 (10.0.10240).
		/// </summary>
		ToastNotification,
		/// <summary>
		/// The app was launched to handle the user interface for account
		/// management. In circumstances where the system would have shown
		/// the default system user interface, it instead has invoked your
		/// app with the UserDataAccountProvider contract. The activation
		/// payload contains information about the type of operation being
		/// requested and all the information necessary to replicate the
		/// system-provided user interface. This activation kind is limited
		/// to 1st party apps. To use this field, you must add the
		/// userDataAccountsProvider capability in your app's package
		/// manifest. For more info see App capability declarations.
		/// Introduced in Windows 10, version 1607 (10.0.14393).
		/// </summary>
		UserDataAccountsProvider,
		/// <summary>
		/// The app was activated as the result of a voice command.
		/// </summary>
		VoiceCommand,
		/// <summary>
		/// The app is a VPN foreground app that was activated by the plugin.
		/// For more details, see VpnChannel.ActivateForeground.
		/// </summary>
		VpnForeground,
		/// <summary>
		/// The app was activated to perform a Wallet operation.
		/// </summary>
		WalletAction,
		/// <summary>
		/// The app was activated by a web account provider.
		/// </summary>
		WebAccountProvider,
		/// <summary>
		/// The app was activated after the app was suspended for a web
		/// authentication broker operation.
		/// </summary>
		WebAuthenticationBrokerContinuation
	}
}

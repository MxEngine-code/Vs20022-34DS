Public Class Form1
    Dim blocos As New List(Of Panel)
    Private Async Sub GerarGrid(ctam As Size, tamanho As Size)
        Timer1.Stop()
        Me.Controls.Clear()
        For x As Integer = 0 To tamanho.Width
            For y As Integer = 0 To tamanho.Height
                Dim pnew As New Panel With {
                .Size = ctam,
                .Location = New Point(50 * x, 50 * y),
                .BackColor = Color.Black
                }
                Me.Controls.Add(pnew)
                Await Task.Delay(3)
                If (Math.Round(tamanho.Width / 2) - 1 = x And Math.Round(tamanho.Height / 2) - 1 = y) Then pnew.BackColor = Color.Green
                blocos.Add(pnew)
            Next
        Next
        Timer1.Start()
    End Sub
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim bnew As New Bloco
        bnew.x = 100
        bnew.y = 100
        bnew.cor = Color.DarkBlue
        listaBlocos.Add(bnew)
        GerarGrid(New Size(50, 50), New Size(Math.Round(Me.Size.Width / 50) + 1, Math.Round(Me.Size.Height / 50) + 1))
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        GerarGrid(New Size(50, 50), New Size(Math.Round(Me.Size.Width / 50) + 1, Math.Round(Me.Size.Height / 50) + 1))
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        For Each i In blocos
            For Each i2 In listaBlocos
                If i2.x * 50 = i.Location.X Then
                    For Each i3 In Me.Controls
                        If (i3 = i) Then i3.BackColor = i2.cor
                    Next
                End If
            Next
        Next
    End Sub
End Class

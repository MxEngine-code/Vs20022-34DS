Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.Devices

Public Class Form1

    Private hue As Integer = 0
    Private inputStart As Integer
    Private caminho As String = "C:\"
    Private Shared Rnd As New Random()
    Private variables As New Dictionary(Of String, Object)
    Private sw As New Stopwatch()
    <DllImport("user32.dll")>
    Private Shared Function ReleaseCapture() As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As Integer, lParam As Integer) As Integer
    End Function

    Private Const WM_NCLBUTTONDOWN As Integer = &HA1
    Private Const HTCAPTION As Integer = 2
    Public Enum TreeSearchMode
        Normal
        DeepWindow
        DeepHidden
        NoWindow
    End Enum
    Private Function ResolverVariaveis(texto As String) As String
        For Each kvp In variables
            texto = texto.Replace($"${kvp.Key}$", kvp.Value.ToString())
        Next
        Return texto
    End Function
    Private Function CalcularExpressao(expr As String) As Object
        Try
            expr = ResolverVariaveis(expr)

            Dim dt As New DataTable()
            Return dt.Compute(expr, "")
        Catch
            Return "ERR"
        End Try
    End Function
    Private Function HSVtoRGB(h As Integer, s As Double, v As Double) As Color
        Dim c = v * s
        Dim x = c * (1 - Math.Abs((h / 60 Mod 2) - 1))
        Dim m = v - c

        Dim r = 0.0, g = 0.0, b = 0.0

        If h < 60 Then r = c : g = x
        If h < 120 AndAlso h >= 60 Then r = x : g = c
        If h < 180 AndAlso h >= 120 Then g = c : b = x
        If h < 240 AndAlso h >= 180 Then g = x : b = c
        If h < 300 AndAlso h >= 240 Then r = x : b = c
        If h >= 300 Then r = c : b = x

        Return Color.FromArgb((r + m) * 255, (g + m) * 255, (b + m) * 255)
    End Function
    Private Sub DesenharGrid(e As PaintEventArgs)
        Dim spacing As Integer = 30
        Dim pen As New Pen(Color.FromArgb(15, Color.Cyan))
        For x = 0 To Me.Width Step spacing
            e.Graphics.DrawLine(pen, x, 0, x, Me.Height)
        Next
        For y = 0 To Me.Height Step spacing
            e.Graphics.DrawLine(pen, 0, y, Me.Width, y)
        Next
    End Sub
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        Dim rect As New Rectangle(10, 10, Me.Width - 20, Me.Height - 20)
        Dim shadowRect As New Rectangle(15, 15, Me.Width - 20, Me.Height - 20)

        Dim path = CriarRegiaoArredondada(rect, 30)
        Dim shadowPath = CriarRegiaoArredondada(shadowRect, 30)

        ' cores neon dinâmicas
        Dim cor1 = HSVtoRGB(hue, 1, 1)
        Dim cor2 = HSVtoRGB((hue + 120) Mod 360, 1, 1)

        ' ===== SOMBRA (profundidade) =====
        For i = 0 To 8
            Using shadowPen As New Pen(Color.FromArgb(15 - i, Color.Black), 20 + i)
                e.Graphics.DrawPath(shadowPen, shadowPath)
            End Using
        Next

        ' ===== FUNDO GRADIENTE ESCURO =====
        Using bgBrush As New Drawing2D.LinearGradientBrush(rect,
        Color.FromArgb(10, 10, 10),
        Color.FromArgb(25, 25, 25),
        90)

            e.Graphics.FillPath(bgBrush, path)
        End Using

        ' ===== GLOW NEON =====
        For i = 0 To 5
            Using glowPen As New Pen(Color.FromArgb(40 - i * 6, cor1), 12 - i * 2)
                e.Graphics.DrawPath(glowPen, path)
            End Using
        Next

        ' ===== BORDA PRINCIPAL =====
        Using brush As New Drawing2D.LinearGradientBrush(rect, cor1, cor2, 45)
            Using pen As New Pen(brush, 2.5F)
                e.Graphics.DrawPath(pen, path)
            End Using
        End Using

        ' GRID hacker
        DesenharGrid(e)
    End Sub
    Private Function CriarRegiaoArredondada(rect As Rectangle, raio As Integer) As Drawing2D.GraphicsPath
        Dim path As New Drawing2D.GraphicsPath()

        path.StartFigure()
        path.AddArc(rect.X, rect.Y, raio, raio, 180, 90)
        path.AddArc(rect.Right - raio, rect.Y, raio, raio, 270, 90)
        path.AddArc(rect.Right - raio, rect.Bottom - raio, raio, raio, 0, 90)
        path.AddArc(rect.X, rect.Bottom - raio, raio, raio, 90, 90)
        path.CloseFigure()

        Return path
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ConfigurarEsteticaHacker()

        If Me.Text = "Form1" Then
            EscreverTexto("--- CHAC... ATAC SYSTEM INITIALIZED ---", True)
            EscreverTexto("Type '?/' for help", False)
        End If

        AppendPrompt()
    End Sub

    Private Sub ConfigurarEsteticaHacker()
        RichTextBox1.Font = New Font("Consolas", 9, FontStyle.Bold)
        RichTextBox1.BackColor = Color.Black

        Dim cores() As Color = {Color.Lime, Color.Green, Color.SpringGreen, Color.MediumSeaGreen}
        RichTextBox1.ForeColor = cores(Rnd.Next(cores.Length))

        RichTextBox1.BorderStyle = BorderStyle.None
        Me.BackColor = Color.Black
        Dim path = CriarRegiaoArredondada(Me.ClientRectangle, 20)
        Me.Region = New Region(path)
    End Sub

    Private Sub AppendPrompt()
        RichTextBox1.AppendText(vbCrLf & $"root@system:{caminho}# ")
        inputStart = RichTextBox1.TextLength
        RichTextBox1.SelectionStart = inputStart
        RichTextBox1.ScrollToCaret()
    End Sub

    Private Async Sub EscreverTexto(texto As String, animado As Boolean)
        If Not animado Then
            RichTextBox1.AppendText(texto & vbCrLf)
            RichTextBox1.ScrollToCaret()
            Return
        End If

        For Each c As Char In texto
            RichTextBox1.AppendText(c)
            Await Task.Delay(10)
        Next

        RichTextBox1.AppendText(vbCrLf)
    End Sub

    Private Sub ExibirHelp()
        EscreverTexto("=== COMMAND LIST (ADVANCED) ===", True)

        EscreverTexto("--- FILE SYSTEM ---", False)
        EscreverTexto("path <dir>          - Change directory", False)
        EscreverTexto("path ..             - Go back directory", False)
        EscreverTexto("path                - Show current path", False)
        EscreverTexto("tree                - List directory", False)
        EscreverTexto("tree -n nome        - Search (no window)", False)
        EscreverTexto("tree -l nome        - Deep search (window)", False)
        EscreverTexto("tree -l-n nome      - Deep hidden search", False)
        EscreverTexto("tree -f / -ff / -fd - Stop rules", False)

        EscreverTexto("--- CONSOLE ---", False)
        EscreverTexto("cls                 - Clear screen", False)
        EscreverTexto("echo <txt>          - Animated text", False)
        EscreverTexto("cons.color <color>  - Change text color", False)

        EscreverTexto("--- SYSTEM ---", False)
        EscreverTexto("sys.info            - System info", False)
        EscreverTexto("ipconfig            - Show IP info", False)
        EscreverTexto("ipconfig /all       - Detailed network info", False)

        EscreverTexto("--- DEV ---", False)
        EscreverTexto("vsc .               - Open VS Code here", False)

        EscreverTexto("?/                  - Show this help", False)
    End Sub

    Public Async Function TreeSearch(
        path As String,
        busca As String,
        modo As TreeSearchMode,
        Optional stopOnFirst As Boolean = False,
        Optional stopFile As Boolean = False,
        Optional stopDir As Boolean = False,
        Optional nivel As Integer = 0
    ) As Task

        Try
            Dim dirs = Directory.GetDirectories(path)
            Dim files = Directory.GetFiles(path)

            For Each d In dirs
                If d.IndexOf(busca, StringComparison.OrdinalIgnoreCase) >= 0 Then
                    fs.Add("[DIR] " & d)

                    If stopOnFirst Or stopDir Then Return
                End If

                If modo <> TreeSearchMode.NoWindow Then
                    Me.Invoke(Sub() EscreverTexto("> DIR: " & d, False)
                                  )
                End If
            Next

            For Each f In files
                If System.IO.Path.GetFileName(f).Contains(busca) Then
                    fs.Add("[FILE] " & f)

                    If stopOnFirst Or stopFile Then Return
                End If

                If modo <> TreeSearchMode.NoWindow Then
                    Me.Invoke(Sub() EscreverTexto("> FILE: " & System.IO.Path.GetFileName(f), False))
                End If
            Next

            Dim tasks As New List(Of Task)

            For Each d In dirs
                Select Case modo

                    Case TreeSearchMode.DeepWindow, TreeSearchMode.DeepHidden
                        Dim nova As Form1 = Nothing

                        Me.Invoke(Sub()
                                      nova = New Form1
                                      nova.Text = "THREAD_" & nivel
                                      nova.Size = New Size(400, 300)

                                      If modo = TreeSearchMode.DeepHidden Then
                                          nova.Opacity = 0
                                      End If

                                      nova.Show()
                                  End Sub)

                        tasks.Add(nova.TreeSearch(d, busca, modo, stopOnFirst, stopFile, stopDir, nivel + 1))

                    Case Else
                        tasks.Add(TreeSearch(d, busca, modo, stopOnFirst, stopFile, stopDir, nivel + 1))
                End Select
            Next

            Await Task.WhenAll(tasks)

        Catch
        End Try

        If nivel > 0 AndAlso modo = TreeSearchMode.DeepWindow Then
            Me.Invoke(Sub() Me.Close())
        End If

    End Function

    Function GerarProximo(atual As String) As String
        Dim alfabeto As String = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Dim cs() As Char = atual.ToCharArray
        Dim i As Integer = cs.Length - 1

        While i >= 0
            Dim index As Integer = alfabeto.IndexOf(cs(i))
            If index < alfabeto.Length - 1 Then
                cs(i) = alfabeto(index + 1)
                Return New String(cs)
            Else
                cs(i) = alfabeto(0)
                i -= 1
            End If
        End While
        Return New String(alfabeto(0), atual.Length + 1)
    End Function
    Public Async Function ProcessCommand(cmd As String) As Task

        If String.IsNullOrWhiteSpace(cmd) Then Return

        Dim partes = cmd.Split(" "c)
        Dim acao = partes(0).ToLower()

        Select Case acao

            Case "cons.clear"
                RichTextBox1.Clear()
                inputStart = 0

            Case "cons.write"
                Dim texto = cmd.Substring(10)
                texto = ResolverVariaveis(texto)
                EscreverTexto(texto, True)

            Case "cons.color"
                Try
                    RichTextBox1.ForeColor = Color.FromName(partes(1))
                Catch
                End Try

            Case "sys.info"
                EscreverTexto("OS: " & Environment.OSVersion.ToString(), False)
                EscreverTexto("CPU: " & Environment.ProcessorCount, False)
                EscreverTexto("USER: " & Environment.UserName, False)


            Case "bt"
                If partes.Length > 1 Then
                    EscreverTexto("Iniciando 3...", False)
                    Await Task.Delay(1000)
                    EscreverTexto("Iniciando 2...", False)
                    Await Task.Delay(1000)
                    EscreverTexto("Iniciando 1...", False)
                    Await Task.Delay(1000)
                    Dim t As DateTime = DateTime.Now
                    Dim atual As String = "at"
                    sw.Start()

                    While sw.Elapsed.TotalSeconds < partes(1)
                        SendKeys.Send("^a{DEL}" & atual & "{ENTER}")
                        atual = GerarProximo(atual)
                    End While
                    Dim L As String

                End If


            Case "path"
                If partes.Length = 1 Then
                    EscreverTexto("Current: " & caminho, False)
                    Return
                End If

                Dim p = cmd.Substring(5).Trim()

                Try
                    If p = ".." Then
                        caminho = Directory.GetParent(caminho).FullName
                    Else
                        Dim novo = If(Path.IsPathRooted(p), p, Path.Combine(caminho, p))
                        If Directory.Exists(novo) Then
                            caminho = novo
                        Else
                            EscreverTexto("Path not found.", False)
                        End If
                    End If
                Catch
                    EscreverTexto("Invalid path.", False)
                End Try

            Case "tree"

                Dim modo As TreeSearchMode = TreeSearchMode.Normal
                Dim busca As String = ""
                Dim stopOnFirst As Boolean = False
                Dim stopFile As Boolean = False
                Dim stopDir As Boolean = False

                If cmd.Contains("-l-n") Then modo = TreeSearchMode.DeepHidden
                If cmd.Contains("-l ") Then modo = TreeSearchMode.DeepWindow
                If cmd.Contains("-n") Then modo = TreeSearchMode.NoWindow

                If cmd.Contains("-ff") Then stopFile = True
                If cmd.Contains("-fd") Then stopDir = True
                If cmd.Contains("-f") Then stopOnFirst = True

                If partes.Length > 1 Then
                    busca = partes.Last()
                End If

                If modo = TreeSearchMode.Normal Then
                    For Each d In Directory.GetDirectories(caminho)
                        EscreverTexto("[DIR] " & d, False)
                    Next

                    For Each f In Directory.GetFiles(caminho)
                        EscreverTexto("[FILE] " & Path.GetFileName(f), False)
                    Next

                    Return
                End If

                fs.Clear()

                Task.Run(Async Function()
                             Await TreeSearch(caminho, busca, modo, stopOnFirst, stopFile, stopDir)

                             Me.Invoke(Sub()
                                           Dim res As New Form1
                                           res.Text = "RESULT"
                                           res.Show()

                                           res.EscreverTexto("=== RESULTADOS ===", True)

                                           For Each item In fs
                                               res.EscreverTexto(item, False)
                                           Next
                                       End Sub)
                         End Function)
            Case "vsc"
                Try
                    Dim alvo As String = caminho
                    If partes.Length > 1 AndAlso partes(1) <> "." Then
                        alvo = partes(1)
                    End If

                    Process.Start("cmd", "/c code """ & alvo & """")
                    EscreverTexto("Opening VS Code...", False)
                Catch
                    EscreverTexto("VS Code not found (check PATH).", False)
                End Try

            Case "ipconfig"
                Try
                    Dim args As String = ""
                    If partes.Length > 1 Then args = partes(1)

                    Dim psi As New ProcessStartInfo()
                    psi.FileName = "cmd.exe"
                    psi.Arguments = "/c ipconfig " & args
                    psi.RedirectStandardOutput = True
                    psi.UseShellExecute = False
                    psi.CreateNoWindow = True

                    Dim p As Process = Process.Start(psi)
                    Dim output As String = p.StandardOutput.ReadToEnd()

                    EscreverTexto(output, False)
                Catch
                    EscreverTexto("Error running ipconfig.", False)
                End Try

            Case "chac"
                If partes.Length < 2 Then
                    EscreverTexto("Usage: chac {file}", False)
                    Return
                End If

                Dim filePath As String = partes(1)

                ' Se não for caminho absoluto, combina com o caminho atual
                If Not Path.IsPathRooted(filePath) Then
                    filePath = Path.Combine(caminho, filePath)
                End If

                ' Verifica se existe
                If Not File.Exists(filePath) Then
                    EscreverTexto("File not found: " & filePath, False)
                    Return
                End If

                ' Lê cada linha e executa como comando
                Try
                    Dim linhas() As String = File.ReadAllLines(filePath)
                    For Each linha In linhas
                        linha = linha.Trim()
                        If Not String.IsNullOrWhiteSpace(linha) Then
                            Await ProcessCommand(linha)
                        End If
                    Next
                Catch ex As Exception
                    EscreverTexto("Error reading file: " & ex.Message, False)
                End Try

            Case "create"
                If partes.Length < 4 Then
                    EscreverTexto("Usage: create name type value", False)
                    Return
                End If

                Dim nome = partes(1)
                Dim tipo = partes(2).ToLower()
                Dim valorRaw = cmd.Substring(cmd.IndexOf(tipo) + tipo.Length).Trim()

                valorRaw = ResolverVariaveis(valorRaw)

                Try
                    Select Case tipo
                        Case "int"
                            variables(nome) = CInt(valorRaw)
                        Case "double"
                            variables(nome) = CDbl(valorRaw)
                        Case "string"
                            variables(nome) = valorRaw.Replace("""", "")
                        Case Else
                            EscreverTexto("Unknown type.", False)
                            Return
                    End Select

                    EscreverTexto($"Variable '{nome}' created.", False)
                Catch
                    EscreverTexto("Error creating variable.", False)
                End Try

            Case "modify"
                If partes.Length < 3 Then Return

                Dim nome = partes(1)

                If Not variables.ContainsKey(nome) Then
                    EscreverTexto("Variable not found.", False)
                    Return
                End If

                Dim valorRaw = cmd.Substring(cmd.IndexOf(nome) + nome.Length).Trim()

                If valorRaw.StartsWith("%calc(") AndAlso valorRaw.EndsWith(")") Then
                    Dim expr = valorRaw.Substring(6, valorRaw.Length - 7)
                    variables(nome) = CalcularExpressao(expr)
                Else
                    valorRaw = ResolverVariaveis(valorRaw)
                    variables(nome) = valorRaw
                End If

                EscreverTexto($"Variable '{nome}' updated.", False)
            Case "?/"
                ExibirHelp()

        End Select

    End Function



    Private Sub RichTextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles RichTextBox1.KeyDown

        If RichTextBox1.SelectionStart < inputStart AndAlso e.KeyCode <> Keys.Left Then
            e.SuppressKeyPress = True
        End If

        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True

            Dim comando = RichTextBox1.Text.Substring(inputStart).Trim()
            ProcessCommand(comando)

            AppendPrompt()
        End If

    End Sub

    Private Sub XToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles XToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub FecharToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FecharToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Private Sub MinimizarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MinimizarToolStripMenuItem.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub MaximizarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MaximizarToolStripMenuItem.Click
        If (Me.WindowState = FormWindowState.Maximized) Then
            MaximizarToolStripMenuItem.Text = "Maximizar"
            Me.WindowState = FormWindowState.Normal
        Else
            MaximizarToolStripMenuItem.Text = "Normal"
            Me.WindowState = FormWindowState.Maximized
        End If
    End Sub

    Private Sub MenuStrip1_MouseDown(sender As Object, e As MouseEventArgs) Handles MenuStrip1.MouseDown
        If e.Button = MouseButtons.Left Then
            ReleaseCapture()
            SendMessage(Me.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0)
        End If
    End Sub

    Private Sub TimerRgb_Tick(sender As Object, e As EventArgs) Handles TimerRgb.Tick
        hue = (hue + 2) Mod 360
        Me.Invalidate()
    End Sub
End Class
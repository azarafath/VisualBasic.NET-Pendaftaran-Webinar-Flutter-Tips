'Mengimport beberapa Library tambahan untuk terkoneksi dengan Firebase
Imports FireSharp.Config
Imports FireSharp.Response
Imports FireSharp.Interfaces
Imports System.Web.Script.Serialization
Imports System.ComponentModel
Imports System.IO
Public Class Form1
    Dim fotoProfileInput As String 'Tempat Menyimpan Sementara Direktori Foto Profile
    Dim hapusKolom As Boolean = True
    Private fconfig As New FirebaseConfig() With
        {
        .AuthSecret = "9gz3hVlkFOxG1NK6I3EqZiouYgeaJjqpSwhvtnGj",
        .BasePath = "https://tubes-pemvis-default-rtdb.asia-southeast1.firebasedatabase.app/"
        }
    Private client As IFirebaseClient


    'Untuk Konversi Image FotoProfile ke Base64 dan kembali ke Image
    Public Function ImageToBase64(image As Image) As String
        Using ms As New MemoryStream()
            Dim Format As System.Drawing.Imaging.ImageFormat = Imaging.ImageFormat.Jpeg
            image.Save(ms, Format)
            Dim imageBytes As Byte() = ms.ToArray()
            Dim base64String As String = Convert.ToBase64String(imageBytes)
            Return base64String
        End Using
    End Function
    Public Function Base64ToImage(base64String As String) As Image
            ' Konversi tipe data Base64 ke Byte 
        Dim imageBytes As Byte() = Convert.FromBase64String(base64String)
        Dim ms As New MemoryStream(imageBytes, 0, imageBytes.Length)

            ' Konversi Byte ke Image 
        ms.Write(imageBytes, 0, imageBytes.Length)
        Dim image__1 As Image = System.Drawing.Image.FromStream(ms, True)
        Return image__1
    End Function
    Sub UbahStateReg(Stat As Boolean)
        txtNama.Enabled = Stat
        txtNoHP.Enabled = Stat
        cmbJK.Enabled = Stat
        txtKota.Enabled = Stat
        txtInstansi.Enabled = Stat
        btnSimpan.Enabled = Stat
        btnClear.Enabled = Stat
    End Sub
    Sub TampilkanData(Stat As Boolean)
        btnEdit.Enabled = Stat
        btnHapus.Enabled = Stat
        btnRefresh.Enabled = Stat
        tabelUserData.Enabled = Stat
    End Sub
    Sub MenampilkanRecord()
        Try
            Dim dtTable As New DataTable
            dtTable.Columns.Add("Nama")
            dtTable.Columns.Add("No HP")
            dtTable.Columns.Add("L/P")
            dtTable.Columns.Add("Kota")
            dtTable.Columns.Add("Instansi")
            dtTable.Columns.Add("Foto", GetType(Image))

            'Pengkondisian Hapus Kolom (Dipanggil Sekali)
            If hapusKolom = True Then
                tabelUserData.Columns.Clear()
                hapusKolom = False
            End If

            Dim TampilRecord = client.Get("PersonDB/") 'Untuk Menyimpan File JSON (Respon dari Firebase)
            Dim myJsonTool As New JavaScriptSerializer
            Dim deserialisasi = myJsonTool.Deserialize(Of Dictionary(Of String, Data))(TampilRecord.Body) 'Ubah String ke Objek

            If deserialisasi IsNot Nothing Then
                For Each dictItem As KeyValuePair(Of String, Data) In deserialisasi
                    dtTable.Rows.Add(dictItem.Value.Nama, dictItem.Value.NoHP, dictItem.Value.JenisKelamin, dictItem.Value.Kota, dictItem.Value.Instansi, Base64ToImage(dictItem.Value.Foto))
                Next
            End If
            tabelUserData.DataSource = dtTable 'Untuk Menampilkan Data ke Tabel dari Source Data (Firebase)

            Dim imageColumn = DirectCast(tabelUserData.Columns("Foto"), DataGridViewImageColumn)
            imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom
            Label10.Text = "Total Pendaftar : " & tabelUserData.RowCount
            btnRefresh.Text = "Refresh"
            TampilkanData(True)
            tabelUserData.ClearSelection()
        Catch ex As Exception
            If ex.Message = "Terjadi Kesalahan." Then
                MessageBox.Show("Tidak dapat terkoneksi ke database, Periksa Koneksi !", "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ElseIf ex.Message = "Referensi objek belum diatur." Then
                Dim dtTable As New DataTable
                dtTable.Columns.Add("Col1")
                dtTable.Columns.Add("Col2")
                dtTable.Columns.Add("Col3")
                dtTable.Columns.Add("Col4")
                dtTable.Columns.Add("Col5")
                dtTable.Columns.Add("Col6", GetType(Image))
                tabelUserData.DataSource = dtTable
                MessageBox.Show("Database Kosong.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                MessageBox.Show(ex.Message, "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
            btnRefresh.Text = "Refresh"
            TampilkanData(True)
        End Try
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            client = New FireSharp.FirebaseClient(fconfig)
            btnRefresh_Click(sender, e)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Tidak dapat terhubung ke Database, Periksa Koneksi Anda", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnSimpan_Click(sender As Object, e As EventArgs) Handles btnSimpan.Click
        If btnSimpan.Text = "Simpan" Then
            'vbCrLf digunakan untuk mengganti baris pada VB Terbaru
            If txtNama.Text = Nothing Then
                MessageBox.Show("Kolom Nama Kosong." & vbCrLf & "Tolong isi kolom tersebut untuk melanjutkan.", "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            If txtNoHP.Text = Nothing Then
                MessageBox.Show("Kolom Nomor HP Kosong." & vbCrLf & "Tolong isi kolom tersebut untuk melanjutkan.", "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            If cmbJK.Text = Nothing Then
                MessageBox.Show("Kolom Jenis Kelamin Kosong." & vbCrLf & "Tolong isi kolom tersebut untuk melanjutkan.", "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            If txtKota.Text = Nothing Then
                MessageBox.Show("Kolom Kota Kosong" & vbCrLf & "Tolong isi kolom tersebut untuk melanjutkan.", "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            If txtInstansi.Text = Nothing Then
                MessageBox.Show("Kolom Instansi Kosong" & vbCrLf & "Tolong isi kolom tersebut untuk melanjutkan.", "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            If fotoProfileInput = Nothing Then
                MessageBox.Show("Foto Profile belum di masukkan." & vbCrLf & "Tolong tambahkan foto untuk melanjutkan.", "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Try
                btnSimpan.Text = "Menyimpan.."
                UbahStateReg(False)
                Dim ImgData As String = ImageToBase64(PictureBox1.Image)

                Dim PD As New Data() With
                    {
                    .Nama = txtNama.Text,
                    .NoHP = txtNoHP.Text,
                    .JenisKelamin = cmbJK.Text,
                    .Kota = txtKota.Text,
                    .Instansi = txtInstansi.Text,
                    .Foto = ImgData
                    }
                Dim save = client.Set("PersonDB/" + txtNoHP.Text, PD) 'Untuk Menyimpan data ke Firebase
                UbahStateReg(True)
                btnSimpan.Text = "Simpan"
                MessageBox.Show("Data Sukses Tersimpan.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

                btnClear_Click(sender, e)
                btnRefresh_Click(sender, e)
            Catch ex As Exception
                If ex.Message = "Terjadi Kesalahan." Then
                    MessageBox.Show("Tidak dapat terkoneksi ke database, periksa kembali koneksi internet anda !", "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Else
                    MessageBox.Show(ex.Message, "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
                UbahStateReg(True)
                btnSimpan.Text = "Simpan"
            End Try

        Else 'Pengkondisian Update Data

            If txtNama.Text = Nothing Then
                MessageBox.Show("Kolom Nama Kosong !." & vbCrLf & "Tolong isi kolom tersebut untuk melanjutkan.", "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            If txtKota.Text = Nothing Then
                MessageBox.Show("Kolom Kota Kosong !." & vbCrLf & "Tolong isi kolom tersebut untuk melanjutkan.", "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            If txtInstansi.Text = Nothing Then
                MessageBox.Show("Kolom Instansi Kosong" & vbCrLf & "Tolong isi kolom tersebut untuk melanjutkan.", "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Try
                btnSimpan.Text = "Memperbarui.."
                UbahStateReg(False)

                Dim ImgData As String = ImageToBase64(PictureBox1.Image)

                Dim PD As New Data() With
                    {
                    .Nama = txtNama.Text,
                    .NoHP = txtNoHP.Text,
                    .JenisKelamin = cmbJK.Text,
                    .Kota = txtKota.Text,
                    .Instansi = txtInstansi.Text,
                    .Foto = ImgData
                    }

                Dim update = client.Update("PersonDB/" + txtNoHP.Text, PD) 'Untuk Update data ke Firebase

                UbahStateReg(True)
                btnSimpan.Text = "Simpan"
                MessageBox.Show("Data Berhasil Diubah.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)
                btnClear_Click(sender, e)
                btnRefresh_Click(sender, e)
            Catch ex As Exception
                If ex.Message = "Terjadi Kesalahan." Then
                    MessageBox.Show("Tidak dapat terkoneksi ke database, periksa kembali koneksi internet anda !", "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Else
                    MessageBox.Show(ex.Message, "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
                UbahStateReg(True)
                btnSimpan.Text = "Simpan"
            End Try
        End If
    End Sub
    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        OpenFileDialog1.FileName = ""
        OpenFileDialog1.Filter = "JPEG (*.jpeg;*.jpg)|*.jpeg;*.jpg"

        If (OpenFileDialog1.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            fotoProfileInput = OpenFileDialog1.FileName
            PictureBox1.ImageLocation = fotoProfileInput
        End If
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        txtNama.Clear()
        txtNoHP.Clear()
        cmbJK.Text = Nothing
        txtKota.Clear()
        txtInstansi.Clear()
        PictureBox1.Image = My.Resources.Pic
        fotoProfileInput = Nothing
        btnSimpan.Text = "Simpan"
    End Sub
    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        btnRefresh.Text = "Merefresh..."
        MenampilkanRecord()
        TampilkanData(True)
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        txtNoHP.Enabled = False
        cmbJK.Enabled = False

        If tabelUserData.SelectedRows.Count = 0 Then
            MessageBox.Show("Tolong Pilih salah satu baris sebelum mengedit.", "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If tabelUserData.SelectedRows.Count > 1 Then
            MessageBox.Show("Anda memilih " & tabelUserData.SelectedRows.Count & " Baris dalam tabel." & vbCrLf & "Fitur Edit Hanya dapat mengedit satu baris.", "Pesan Erorr", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        If tabelUserData.SelectedRows.Count = 1 Then
            txtNama.Text = tabelUserData.SelectedRows(0).Cells("Nama").Value
            txtNoHP.Text = tabelUserData.SelectedRows(0).Cells("No HP").Value
            cmbJK.Text = tabelUserData.SelectedRows(0).Cells("L/P").Value
            txtKota.Text = tabelUserData.SelectedRows(0).Cells("Kota").Value
            txtInstansi.Text = tabelUserData.SelectedRows(0).Cells("Instansi").Value
            PictureBox1.Image = tabelUserData.Rows(tabelUserData.SelectedRows(0).Index).Cells("Foto").Value
            btnSimpan.Text = "Update"
        End If
    End Sub

    Private Sub btnHapus_Click(sender As Object, e As EventArgs) Handles btnHapus.Click
        Try
            If tabelUserData.SelectedRows.Count = 0 Then
                MessageBox.Show("Tolong Pilih Baris yang akan dihapus .", "Pesan Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            If MsgBox("Apakah kamu yakin menghapus data ini ?", MsgBoxStyle.Question + MsgBoxStyle.OkCancel, "Confirmation") = MsgBoxResult.Cancel Then Return

            TampilkanData(False)

            For Each row As DataGridViewRow In tabelUserData.SelectedRows
                If row.Selected = True Then
                    Dim delete = client.Delete("PersonDB/" & row.DataBoundItem(1).ToString)
                End If
            Next

            MessageBox.Show("Data Sukses Terhapus.", "Informasi", MessageBoxButtons.OK, MessageBoxIcon.Information)

            TampilkanData(True)

            btnRefresh_Click(sender, e)
        Catch ex As Exception
            If ex.Message = "Terjadi Kesalahan." Then
                MessageBox.Show("Tidak dapat terhubung ke database, Periksa Koneksi Internet !", "Pesan Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                MessageBox.Show(ex.Message, "Pesan Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

            TampilkanData(True)
        End Try
    End Sub

    Private Sub btnKeluar_Click(sender As Object, e As EventArgs) Handles btnKeluar.Click
        Me.Close()
    End Sub
End Class

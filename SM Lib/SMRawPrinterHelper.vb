Imports System
Imports System.Drawing
'Imports System.Drawing.Printing
'Imports System.Windows.Forms
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Collections

Public Class SMRawPrinterHelper
    ' Structure and API declarations:
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi)>
    Public Class DOCINFOA
        <MarshalAs(UnmanagedType.LPStr)>
        Public pDocName As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public pOutputFile As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public pDataType As String
    End Class
    <DllImport("winspool.Drv", EntryPoint:="OpenPrinterA",
    SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True,
    CallingConvention:=CallingConvention.StdCall)>
    Public Shared Function OpenPrinter(<MarshalAs(UnmanagedType.LPStr)>
    szPrinter As String, ByRef hPrinter As IntPtr, pd As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="ClosePrinter",
    SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Public Shared Function ClosePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="StartDocPrinterA",
    SetLastError:=True, CharSet:=CharSet.Ansi, ExactSpelling:=True,
    CallingConvention:=CallingConvention.StdCall)>
    Public Shared Function StartDocPrinter(hPrinter As IntPtr, level As Int32,
    <[In](), MarshalAs(UnmanagedType.LPStruct)> di As DOCINFOA) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="EndDocPrinter",
    SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Public Shared Function EndDocPrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="StartPagePrinter",
    SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Public Shared Function StartPagePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="EndPagePrinter",
    SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Public Shared Function EndPagePrinter(hPrinter As IntPtr) As Boolean
    End Function

    <DllImport("winspool.Drv", EntryPoint:="WritePrinter",
    SetLastError:=True, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)>
    Public Shared Function WritePrinter(hPrinter As IntPtr, pBytes As IntPtr,
    dwCount As Int32, ByRef dwWritten As Int32) As Boolean
    End Function

    Private hPrinter As New IntPtr(0)
    Private di As New DOCINFOA()
    Private PrinterOpen As Boolean = False

    Public ReadOnly Property PrinterIsOpen As Boolean
        Get
            PrinterIsOpen = PrinterOpen
        End Get
    End Property

    Public Function OpenPrint(szPrinterName As String) As Boolean
        If PrinterOpen = False Then
            di.pDocName = "Receipt"
            di.pDataType = "RAW"

            If OpenPrinter(szPrinterName.Normalize(), hPrinter, IntPtr.Zero) Then
                ' Start a document.
                If StartDocPrinter(hPrinter, 1, di) Then
                    If StartPagePrinter(hPrinter) Then
                        PrinterOpen = True
                    End If
                End If
            End If
        End If

        OpenPrint = PrinterOpen
    End Function

    Public Sub ClosePrint()
        If PrinterOpen Then
            EndPagePrinter(hPrinter)
            EndDocPrinter(hPrinter)
            ClosePrinter(hPrinter)
            PrinterOpen = False
        End If
    End Sub

    Public Function SendStringToPrinter(szPrinterName As String, szString As String) As Boolean
        If PrinterOpen Then
            Dim pBytes As IntPtr
            Dim dwCount As Int32
            Dim dwWritten As Int32 = 0

            dwCount = szString.Length

            pBytes = Marshal.StringToCoTaskMemAnsi(szString)

            SendStringToPrinter = WritePrinter(hPrinter, pBytes, dwCount, dwWritten)

            Marshal.FreeCoTaskMem(pBytes)
        Else
            SendStringToPrinter = False
        End If
    End Function

    Public Function SendToPrinter(szPrinterName As String, arData() As Byte, length As Integer) As Integer

        If (PrinterIsOpen) Then
            Dim dwWritten As Int32 = 0
            Dim pBytes As IntPtr

            pBytes = Marshal.UnsafeAddrOfPinnedArrayElement(arData, 0)


            WritePrinter(hPrinter, pBytes, length, dwWritten)
            SendToPrinter = dwWritten

        Else
            SendToPrinter = False
        End If

    End Function


End Class
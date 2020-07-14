using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MBStore_MVP.Model
{
    class FTPConnector
    {
        private string id = "ftp_bit";
        private string pwd = "dlqtkgkwk12#$";

        // FTP 서버 접속 함수
        private FtpWebResponse Connect(String url, string method, Action<FtpWebRequest> action = null)
        {
            // WebRequest 클래스를 이용해 접속하기 때문에 객체를 가져온다. (FtpWebRequest로 변환)
            var request = WebRequest.Create(url) as FtpWebRequest;
            // Binary 형식으로 사용한다.
            request.UseBinary = true;
            // FTP 메소드 설정(아래에 별도 설명)
            request.Method = method;
            // 로그인 인증
            request.Credentials = new NetworkCredential(id, pwd);
            // request.GetResponse()함수가 호출되면 실제적으로 접속이 되기 때문에, 그전에 설정할 callback 함수 호출
            if (action != null)
            {
                action(request);
            }
            // 접속해서 WebResponse함수를 가져온다.
            return request.GetResponse() as FtpWebResponse;
        }

        private void UploadFileList(String url, string source)
        {
            // 업로드할 경로의 속성을 구한다.
            var attr = File.GetAttributes(source);
            // 만약 디렉토리라면..
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                // 디렉토리 정보를 가져온다.
                DirectoryInfo dir = new DirectoryInfo(source);
                // 디렉토리 안의 파일 리스트를 가져온다.
                foreach (var item in dir.GetFiles())
                {
                    // 파일을 업로드한다.
                    UploadFileList(url + "/" + item.Name, item.FullName);
                }
                // 디렉토리 안의 하위 디렉토리 리스트를 가져온다.
                foreach (var item in dir.GetDirectories())
                {
                    try
                    {
                        // ftp에 디렉토리를 생성한다.
                        Connect(url + "/" + item.Name, WebRequestMethods.Ftp.MakeDirectory).Close();
                    }
                    catch (WebException)
                    {
                        // 만약에 ftp에 디렉토리가 존재한다면 에러가 날 것이다.
                    }
                    // 디렉토리를 업로드한다.(재귀 함수 호출)
                    UploadFileList(url + "/" + item.Name, item.FullName);
                }
            }
            else
            {
                // 디렉토리가 아닌 파일을 경우인데, 파일의 stream을 취득한다.
                using (var fs = File.OpenRead(source))
                {
                    // 파일을 업로드한다.
                    Connect(url, WebRequestMethods.Ftp.UploadFile, (req) =>
                    {
                        // 파일 크기 설정
                        req.ContentLength = fs.Length;
                        // GetResponse()가 호출되기 전에 request스트림에 파일 binary를 넣는다.
                        using (var stream = req.GetRequestStream())
                        {
                            fs.CopyTo(stream);
                        }
                    }).Close();
                    // respose 객체를 닫는다.
                }
            }
        }
    }
}


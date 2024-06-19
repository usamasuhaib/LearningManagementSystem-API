namespace School_Management_API.Helpers
{
    public static class EmailBody
    {

        public static string resetPasswordBody(string userName, string email, string link)
        {
            return $@"


               <html>
                    <body>

               <h3>Hi ""{userName}""</h3>

                    <p>
                                        Click the link below to reset your password: <br>

                                        <a href=""{link}"">Reset Password</a> <br>

                                        This link will expire in 24 hours.<br>

                                        Sincerely,<br>

                                        Your App Team"";
                   </p>

                    </body>
            </html>";
        }



        public static string adminEmailConfirmBody(string userName, string link)
        {
            return $@"


               <html>
                    <body>

                <h3>Dear Admin ""{userName}""</h3>

                    <p>
                                        Your Account is created succesfully on School SMS  <br>

                                       Click the link below to Confirm Your Email: <br>

                                        <a href=""{link}"">Confirm Password</a> <br>

                                        This link will expire in 24 hours.<br>

                                        Sincerely,<br>

                                        Usama Suhaib, <br>
                                        Web Developer"";
                   </p>

                    </body>
            </html>";
        }






        public static string studnetEmailConfirmBody(string userName, string link)
        {
            return $@"


               <html>
                    <body>

               <h3>Dear Student ""{userName}""</h3>

                    <p>
                                        Your Account is created succesfully on School SMS  <br>

                                       Click the link below to Confirm Your Email: <br>

                                        <a href=""{link}"">Confirm Password</a> <br>

                                        This link will expire in 24 hours.<br>

                                        Your Default Password is : Student@1234 <br>

                                        Sincerely,<br>

                                        Usama Suhaib, <br>
                                        Web Developer"";
                   </p>

                    </body>
            </html>";
        }

        public static string tacherEmailConfirmBody(string userName, string link)
        {
            return $@"


               <html>
                    <body>

               <h4>Respected ""{userName}""</h4>

                    <p>
                                        Your Account is created succesfully on School LMS, your Role is Teacher <br>

                                        <br>                                       
                                        Click the link below to Confirm Your Email: <br>

                                        <a href=""{link}"">Confirm Password</a> <br>

                                        This link will expire in 24 hours.<br>

                                        <br> 
                                        Your Default Password is : Teacher@1234 <br>

                                        Sincerely,<br>

                                        Usama Suhaib, <br>
                                        Web Developer"";
                   </p>

                    </body>
            </html>";
        }


    }
}




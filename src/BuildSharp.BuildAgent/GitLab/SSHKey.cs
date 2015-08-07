using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace BuildSharp.GitLab
{
    public static class SSHKey
    {
        /// <summary>
        /// Generate an SSH keypair.
        /// </summary>
        public static void GenerateSSHKeyPair()
        {
            var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            // We need both, the Public and Private Key
            // Public Key to send to Gitlab
            // Private Key to connect to Gitlab later
            if (File.Exists(profile + @"\.ssh\id_rsa.pub") &&
                File.Exists(profile + @"\.ssh\id_rsa"))
                return;

            CreateKeyPair(profile);

            while (!File.Exists(profile + @"\.ssh\id_rsa.pub") &&
                   !File.Exists(profile + @"\.ssh\id_rsa"))
            {
                Thread.Sleep(1000);
            }

            Console.WriteLine("SSH Key generated successfully!");
        }

        private static void CreateKeyPair(string profile)
        {
            try
            {
                Directory.CreateDirectory(profile + @"\.ssh");
            }
            catch (Exception)
            {
            }

            var p = new Process
            {
                StartInfo =
                {
                    FileName = "ssh-keygen",
                    Arguments = "-t rsa -f " + profile + @"\.ssh\id_rsa -N "
                }
            };
            p.Start();

            Console.WriteLine();
            Console.WriteLine("Waiting for SSH Key to be generated ...");

            p.WaitForExit();
        }

        /// <summary>
        /// Get the public key
        /// </summary>
        /// <returns></returns>
        public static string GetPublicKey()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return File.Exists(userProfile + @"\.ssh\id_rsa.pub") ?
                File.ReadAllText(userProfile + @"\.ssh\id_rsa.pub") : null;
        }
    }
}

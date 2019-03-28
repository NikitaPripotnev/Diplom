using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace osp
{
    /* 
     * Реализация обработки исключений в программе.
     * В текущей реализации вызывается окошко с сообщением об ошибке. 
     * При необходимости, можно сделать вывод в консоль или же писать в файл. 
     */

    class ErrorHandle
    {
        
        /* Просто вывод сообщения. */
        static public void DoHandle(string message)
        {
            DoHandle(message, "Ошибка");
        }

        /* Вывод сообщения с возможностью задать заголовок. */
        static public void DoHandle(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

}
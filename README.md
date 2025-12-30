# ScrMatrix
Компактная утилита, написанная на C#, которая позволяет мгновенно менять громкость звука и яркость монитора с помощью колесика мыши.
Основные функции:
 * Звуковая Матрица: Удерживайте Alt + прокрутка колесика для изменения системной громкости.
 * Матрица Экрана: Удерживайте Ctrl + прокрутка колесика для изменения яркости.
 * Легкость: Программа весит менее 50 КБ и потребляет минимум оперативной памяти.
Версии программы:
 * Gamma Edition (Рекомендуемая): Работает на любых мониторах и видеокартах. Затемняет экран программно.
 * WMI Edition: Работает на ноутбуках, управляя физической подсветкой матрицы.
Как собрать (компиляция):
Вам не нужно устанавливать Visual Studio. Используйте встроенный компилятор Windows:
 * Сохраните код в файл ScrMatrix.cs.
 * Откройте командную строку (CMD) в папке с файлом.
 * Выполните команду:
   ​`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /out:ScrMatrix.exe ScrMatrix.cs​`

   (Для WMI-версии добавьте параметр ​`/r:System.Management.dll​`)
Автозагрузка:
Чтобы скрипт работал всегда:
 * Нажмите Win + R, введите ​`shell:startup​`.
 * Поместите ярлык созданного .exe файла в открывшуюся папку.


**ScrMatrix** is a lightweight C# utility that allows you to instantly change system volume and monitor brightness using your mouse wheel.
Key Features:
 * Sound Matrix: Hold Alt + scroll the mouse wheel to change system volume.
 * Screen Matrix: Hold Ctrl + scroll the mouse wheel to adjust brightness.
 * Performance: The app is smaller than 50 KB and uses minimal RAM.
Available Versions:
 * Gamma Edition (Recommended): Works on all monitors and GPUs. It dims the screen programmatically via gamma ramp.
 * WMI Edition: Best for laptops. It controls the physical backlight of the display.
How to Build (Compilation):
No need to install Visual Studio. You can use the built-in Windows compiler:
 * Save the code as ScrMatrix.cs.
 * Open Command Prompt (CMD) in the source folder.
 * Run the following command:
   ​`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /out:ScrMatrix.exe ScrMatrix.cs​`

   (For the WMI version, add the ​`/r:System.Management.dll​` flag)
Startup:
To keep the script running at all times:
 * Press Win + R, type ​`shell:startup​`.
 * Place a shortcut of your .exe file into the folder that opens.

import java.io.BufferedWriter;
import java.io.FileWriter;

String ApplicationName = "";
String ApplicationID = "";
String rootFolder = "";
String fileName = "";

if (bsh.args.length==4){
    ApplicationName = bsh.args[0];
    ApplicationID = bsh.args[1];
    rootFolder = bsh.args[2];
    fileName = bsh.args[3];
}else{
    System.out.println("missing some data");
}
System.out.println(ApplicationName + ":" + fileName);
FileWriter logFile = new FileWriter("c:\\temp\\sample.log");
BufferedWriter log = new BufferedWriter(logFile);
log.write("rewrite of file");
log.newLine();
log.flush();
log.close();
System.out.println("You called a beanshell rule again");
Thread.sleep(3000);
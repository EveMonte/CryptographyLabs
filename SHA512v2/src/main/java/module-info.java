module com.example.sha512 {
    requires javafx.controls;
    requires javafx.fxml;


    opens com.example.sha512 to javafx.fxml;
    exports com.example.sha512;
}
package com.example.sha512;

import javafx.fxml.FXML;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.stage.Stage;

import javafx.fxml.FXMLLoader;

import java.io.IOException;


public class Application extends javafx.application.Application {

    @FXML
    public Button chooseBtn;

    private String filePath;

    @Override
    public void start(Stage stage) throws IOException {
        FXMLLoader fxmlLoader = new FXMLLoader(Application.class.getResource("lyra2v2-view.fxml"));

        stage.setScene(new Scene(fxmlLoader.load(), 600, 400));
        stage.setResizable(false);
        stage.show();

//        FXMLLoader fxmlLoader = new FXMLLoader(HelloApplication.class.getResource("lyra2v2-view.fxml"));
//        Scene scene = new Scene(fxmlLoader.load(), 600, 400);
//        stage.setResizable(false);
//        stage.setScene(scene);
//        stage.show();
    }

    public static void main(String[] args) {
        launch();
    }
}
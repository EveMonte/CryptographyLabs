package com.example.sha512;


import java.io.File;
import java.io.IOException;
import java.net.URL;
import java.util.ResourceBundle;
import javafx.fxml.FXML;
import javafx.scene.control.*;



public class Controller {

    public static int BLOCK_LEN_INT64 = 12;
    public static int N_COLS = 256;
    public static String SPONGE = "blake2b";
    public static int HALF_ROUNDS = 12;
    public static int FULL_ROUNDS = 12;

    @FXML
    public ComboBox sponge;

    @FXML
    private ResourceBundle resources;

    @FXML
    private URL location;

    @FXML
    private Spinner<Integer> bytes;

//    @FXML
//    private Button hashIt;

    @FXML
    private Label hashedPassword;

    @FXML
    private Label error;

    @FXML
    private Spinner<Integer> memCost;


    @FXML
    private TextField password;

    @FXML
    private TextField saltInp;

    @FXML
    private Spinner<Integer> timeCost;


    public void RunLyra2() {
        if (!SPONGE.equals("blake2b") && !SPONGE.equals("blamka") && !SPONGE.equals("half-round-blamka")) {
            System.err.println("--sponge must be one of: blake2b, blamka or half-blamka");
            System.err.println("Instead, you specified --sponge " + SPONGE);

            return;
        }
        if(password.getText().isEmpty() || saltInp.getText().isEmpty()){
            error.setVisible(true);
            return;
        }
        else
        {
            error.setVisible(false);
        }
        SPONGE = sponge.getValue().toString();
        System.out.println(bytes.getValue());
        LyraParams params = new LyraParams(bytes.getValue(), timeCost.getValue(), memCost.getValue(),
                N_COLS, SPONGE,
                FULL_ROUNDS, HALF_ROUNDS,
                BLOCK_LEN_INT64
        );

        byte[] hash = new byte[bytes.getValue()];
        byte[] pass = password.getText().getBytes();
        byte[] salt = saltInp.getText().getBytes();


        Lyra2.phs(hash, pass, salt, params);

        System.out.println("Output:");
        hashedPassword.setText(echo.bytes(hash, hash.length));
    }

    @FXML
    void initialize() {
        SpinnerValueFactory<Integer> timeCostValueFactory = //
                new SpinnerValueFactory.IntegerSpinnerValueFactory(1, 1000, 1);
        SpinnerValueFactory<Integer> memCostValueFactory = //
                new SpinnerValueFactory.IntegerSpinnerValueFactory(3, 1000, 3);
        SpinnerValueFactory<Integer> bytesCostValueFactory = //
                new SpinnerValueFactory.IntegerSpinnerValueFactory(1, 24, 6);

        sponge.getItems().addAll("blake2b",
                "blamka",
                "half-round-blamka");
        sponge.getSelectionModel().selectFirst();
        timeCost.setValueFactory(timeCostValueFactory);
        memCost.setValueFactory(memCostValueFactory);
        bytes.setValueFactory(bytesCostValueFactory);

    }
}
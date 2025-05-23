CREATE TABLE admin (
admin varchar(255) PRIMARY KEY,
şifre varchar(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS accountant_normalized (
transaction_id SERIAL PRIMARY KEY,
reservation_id INT REFERENCES reservations(reservation_id),
customer_id INT REFERENCES(customer_id),
fee REAL NOT NULL,
transaction_date TIMESTAMP NOT NULL
);


CREATE TABLE IF NOT EXISTS customers_normalized (
customer_id SERIAL PRIMARY KEY,
isim_soyisim VARCHAR(255) NOT NULL );


CREATE TABLE IF NOT EXISTS reservations_normalized(
reservation_id SERIAL PRIMARY KEY,
oda_numarasi INT NOT NULL REFERENCES room(oda_numarasi),
customer_id INT NOT NULL REFERENCES customers(customer_id),
ucret REAL NOT NULL,
giris_tarihi DATE NOT NULL,
cikis_tarihi DATE NOT NULL
);

CREATE TABLE IF NOT EXISTS reservation_expenses_normalized (
    reservation_id INT NOT NULL REFERENCES reservations_normalized(reservation_id),
    expense_amount DOUBLE PRECISION NOT NULL,
    PRIMARY KEY (reservation_id)
);


CREATE TABLE room_normalized (
oda_numarasi INT PRIMARY KEY,
base_price DOUBLE PRECISION NOT NULL
);

CREATE TABLE IF NOT EXISTS room_expenses_normalized (
oda_numarasi INT NOT NULL references room_normalized(oda_numarasi),
expense_type VARCHAR(50) NOT NULL,
expense_amount DOUBLE PRECISION NOT NULL,
PRIMARY KEY (oda_numarasi, expense_type)
);



 DECLARE reservation_count INTEGER; 
BEGIN
  SELECT COUNT(*) INTO reservation_count
  FROM reservations_normalized
  WHERE oda_numarasi = room_number
    AND (
      (start_date BETWEEN giris_tarihi AND cikis_tarihi) OR
      (end_date BETWEEN giris_tarihi AND cikis_tarihi) OR
      (giris_tarihi BETWEEN start_date AND end_date)
    );

  RETURN reservation_count;
END;

DECLARE
    occupancy_rate DOUBLE PRECISION;
    base_price DOUBLE PRECISION;
    calculated_price DOUBLE PRECISION;
    duration INT;
BEGIN
    -- Oda numarasına göre temel fiyatı alıyoruz
    SELECT r.base_price
    INTO base_price
    FROM room_normalized r
    WHERE r.oda_numarasi = roomnumber;

    -- Giriş tarihinden sonraki 7 gün içindeki doluluk oranını hesaplıyoruz
    SELECT
        (COUNT(*) * 100.0) / (SELECT COUNT(*) FROM room_normalized) -- Total rooms
    INTO occupancy_rate
    FROM reservations_normalized r
    WHERE r.giris_tarihi < giristarihi + INTERVAL '7 days' 
      AND r.cikis_tarihi > giristarihi; -- Check for overlapping reservations

    -- Doluluk oranına göre fiyat artışı
    IF occupancy_rate >= 10 THEN
        base_price := base_price * 1.55;
    ELSIF occupancy_rate >= 5 THEN
        base_price := base_price * 1.30;
    END IF;

    -- Süreyi hesaplıyoruz (giriş ve çıkış tarihından)
    duration := (cikistarihi - giristarihi); -- Duration in days

    -- Yeni fiyatı hesaplıyoruz
    calculated_price := base_price * duration;

    -- Hesaplanan fiyatı döndür
    RETURN calculated_price;
END;


DECLARE
    total_fee DOUBLE PRECISION := 0;
    abase_price DOUBLE PRECISION;
    achildren_fee DOUBLE PRECISION := 0;
    abreakfast_fee DOUBLE PRECISION := 0;
    aminibar_fee DOUBLE PRECISION := 0;
    aroom_service_fee DOUBLE PRECISION := 0;
BEGIN
    -- Oda numarasına göre temel fiyatı alıyoruz
    SELECT r.base_price
    INTO abase_price
    FROM room_normalized r
    WHERE r.oda_numarasi = wroom_number;

    -- Ücretleri room_expenses_normalized tablosundan çekiyoruz
    SELECT re.expense_amount
    INTO achildren_fee
    FROM room_expenses_normalized re
    WHERE re.oda_numarasi = wroom_number AND re.expense_type = 'children_fee';

    SELECT re.expense_amount
    INTO abreakfast_fee
    FROM room_expenses_normalized re
    WHERE re.oda_numarasi = wroom_number AND re.expense_type = 'breakfast_fee';

    SELECT re.expense_amount
    INTO aminibar_fee
    FROM room_expenses_normalized re
    WHERE re.oda_numarasi = wroom_number AND re.expense_type = 'minibar_fee';

    SELECT re.expense_amount
    INTO aroom_service_fee
    FROM room_expenses_normalized re
    WHERE re.oda_numarasi = wroom_number AND re.expense_type = 'room_service_fee';

    -- NULL değerleri kontrol ediyoruz ve 0 olarak atıyoruz
    IF abase_price IS NULL THEN
        abase_price := 0;
    END IF;
    IF achildren_fee IS NULL THEN
        achildren_fee := 0;
    END IF;
    IF abreakfast_fee IS NULL THEN
        abreakfast_fee := 0;
    END IF;
    IF aminibar_fee IS NULL THEN
        aminibar_fee := 0;
    END IF;
    IF aroom_service_fee IS NULL THEN
        aroom_service_fee := 0;
    END IF;

    -- Toplam ücreti hesaplıyoruz
    total_fee := achildren_fee + abreakfast_fee + aminibar_fee + aroom_service_fee - 100;

    -- Toplam ücreti döndürüyoruz
    RETURN total_fee;
END;

BEGIN
    RETURN QUERY
    SELECT 
        a.transaction_id,        -- 'a' table's transaction_id
        a.reservation_id,        -- 'a' table's reservation_id
        a.customer_id,           -- 'a' table's customer_id
        a.fee,                   -- 'a' table's fee
        a.transaction_date       -- 'a' table's transaction_date
    FROM 
        accountant_normalized a;  -- Using the accountant_normalized table as alias 'a'
END;



BEGIN
    RETURN QUERY
    SELECT 
        r.reservation_id, 
        c.customer_id, 
        c.isim_soyisim, 
        r.oda_numarasi, 
        re.expense_amount, 
        r.giris_tarihi, 
        r.cikis_tarihi
    FROM 
        reservations_normalized r
    JOIN 
        customers_normalized c ON r.customer_id = c.customer_id
    JOIN 
        reservation_expenses_normalized re ON r.reservation_id = re.reservation_id;
END;


--getting the reservation and customer id for checkout match
BEGIN
    -- Müşteri ID'sini al
    SELECT customer_id 
    INTO p_customer_id
    FROM customers_normalized
    WHERE isim_soyisim = p_customer_name
    LIMIT 1;

    -- Eğer müşteri bulunamazsa hata fırlat
    IF p_customer_id IS NULL THEN
        RAISE EXCEPTION 'Müşteri bulunamadı: %', p_customer_name;
    END IF;

    -- Rezervasyon ID'sini al
    SELECT reservation_id 
    INTO p_reservation_id
    FROM reservations_normalized
    WHERE oda_numarasi = p_room_number
    AND customer_id = p_customer_id;

    -- Eğer rezervasyon bulunamazsa hata fırlat
    IF p_reservation_id IS NULL THEN
        RAISE EXCEPTION 'Rezervasyon bulunamadı: oda numarası = %, müşteri = %', p_room_number, p_customer_name;
    END IF;

    RETURN NEXT;
END;

--getting the room expenses information
BEGIN
    RETURN QUERY
    SELECT 
        r.oda_numarasi,           -- Room number
        r.base_price,             -- Base price from room_normalized
        e.expense_type,           -- Expense type from room_expenses_normalized
        e.expense_amount          -- Expense amount from room_expenses_normalized
    FROM 
        room_normalized r
    JOIN 
        room_expenses_normalized e
    ON 
        r.oda_numarasi = e.oda_numarasi;  -- Join on oda_numarasi (room number)
END;


--reservation insert
DECLARE
    p_reservation_id INTEGER;
BEGIN
    -- Insert the reservation and return the generated reservation_id
    INSERT INTO reservations_normalized (oda_numarasi, customer_id, giris_tarihi, cikis_tarihi) 
    VALUES (selected_room_number, customer_id, giris_tarihi, cikis_tarihi) 
    RETURNING reservation_id INTO p_reservation_id;

    -- Return the generated reservation_id
    RETURN p_reservation_id;
END;

--deleting accountant record
BEGIN
    -- Kayıtları sil
    DELETE FROM accountant_normalized
    WHERE customer_id = p_customer_id AND reservation_id = p_reservation_id;

    -- İşlem sonrası bilgi döndürebilir veya log tutabilirsiniz (isteğe bağlı)
    RAISE NOTICE 'Accountant record deleted for customer_id: %, reservation_id: %', p_customer_id, p_reservation_id;
END;

--inserting reservation expenses
BEGIN
    INSERT INTO reservation_expenses_normalized (reservation_id, expense_amount)
    VALUES (p_reservation_id, p_expense_amount);
END;

BEGIN
    INSERT INTO reservation_expenses_normalized (reservation_id, expense_amount)
    VALUES (p_reservation_id, p_expense_amount);
END;


--accountant logs
DECLARE
    v_customer_id INT;
BEGIN
    -- Fetch customer_id from reservations_normalized table using reservation_id from the NEW record
    SELECT customer_id INTO v_customer_id
    FROM reservations_normalized
    WHERE reservation_id = NEW.reservation_id;

    -- Insert into accountant_normalized table
    INSERT INTO accountant_normalized (reservation_id,customer_id, fee, transaction_date)
    VALUES (NEW.reservation_id,v_customer_id, NEW.expense_amount, CURRENT_TIMESTAMP);

    RETURN NEW;
END;


--Total expenses update trigger
BEGIN
    -- Harcama eklendiğinde veya değiştirildiğinde toplam harcamayı güncelle
    UPDATE accountant_normalized
    SET fee = (
        SELECT SUM(expense_amount)
        FROM reservation_expenses_normalized
        WHERE reservation_id = NEW.reservation_id
    )
    WHERE reservation_id = NEW.reservation_id;

    RETURN NEW;
END;

--Daily earnings view
 SELECT res.oda_numarasi,
    res.giris_tarihi AS date1,
    res.cikis_tarihi AS date2,
    sum(a.fee) AS toplamkazanc
   FROM reservations_normalized res
     JOIN accountant_normalized a ON res.reservation_id = a.reservation_id
  GROUP BY res.oda_numarasi, res.giris_tarihi, res.cikis_tarihi;
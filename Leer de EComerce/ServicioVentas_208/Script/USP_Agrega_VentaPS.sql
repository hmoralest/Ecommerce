If Exists(Select * from sysobjects Where name = 'USP_Agrega_VentaPS' And type = 'P')
	Drop Procedure USP_Agrega_VentaPS
GO

-- ==========================================================================================
-- Author		: Henry Morales
-- Create date	: 13/03/2018
-- Description	: Se utiliza para ingresar datos traidos desde PrestaShop (E-Commerce)
-- ==========================================================================================
/*
	Exec USP_Agrega_VentaPS 'B03000024649', '50005', '03', '03', 'B030', '00024649', '20180313', '19:28:59', 'Henry', '', 'Morales', '', '03', '99999999', '4444444', 'usu', 'vend', 'a0b7rvXm73snju33bo8V9tq38Fo=', '01', '1', '32', '9F', '20180313', 3, 208.4000, 0.0000, 245.9000, 37.5000, '8899998|9999997', '39|00', '1|2', '2|1', '0000|0000', '5|5', '101.6000|5.2000', '0.0000|0.0000'
*/

CREATE Procedure USP_Agrega_VentaPS(
	@validavta			Varchar(max),
	@entidad			Varchar(max),

	@tipo				Varchar(2),

	@tipo_doc			Varchar(2),
	@tipo_doc_sunat		Varchar(2),
	@serie_doc			Varchar(4),
	@numero_doc			Varchar(8),
	@ven_fecha			Varchar(max),
	@ven_hora			Varchar(max),
	@cod_ref			Varchar(max),
	@ser_ref			Varchar(max),
	@num_ref			Varchar(max),
	@pri_nom_cli		Varchar(max),
	@seg_nom_cli		Varchar(max),
	@pri_ape_cli		Varchar(max),
	@seg_ape_cli		Varchar(max),
	@dir_cli			Varchar(max),
	@ruc_cli			Varchar(13),
	@telf_cli			Varchar(max),
	@usu_crea			Varchar(max),
	@cod_vend			Varchar(max),
	@codigo				Varchar(max),
	@moneda				Varchar(2),
	@tipo_camb			Varchar(max),
	@doc_pago			Varchar(max),
	@forma_pago			Varchar(max),
	@fec_pago			Varchar(max),
	@tot_cant			Int,
	@tot_precio_sigv	Decimal(18,2),
	@tot_dcto_sigv		Decimal(18,2),
	@total_venta		Decimal(18,2),
	@tot_igv			Decimal(18,2),
        
	@articulos			Varchar(max),
	@tallas				Varchar(max),
	@items				Varchar(max),
	@cant_artic			Varchar(max),
	@alm_tien			Varchar(max),
	@seccion			Varchar(max),
	@prec_artic			Varchar(max),
	@dcto_artic			Varchar(max)
)
AS 
BEGIN

	BEGIN TRY
		BEGIN TRAN ins_Venta

		Declare @codigo_nota	Varchar(8),
				@codigo_fact	Varchar(8),
				@precio_cigv	Decimal(18,2),
				@total_sigv		Decimal(18,2),

				@Item			Varchar(3),
				@contador		Int


		Set @codigo_fact = Right('00000000'+ Convert(Varchar, (ISNULL((Select MAX(FC_NINT) From Ffactc_ecom Where COD_ENTID = @entidad),0)+1) ),8);
		Set @codigo_nota = Right('00000000'+ Convert(Varchar, (ISNULL((Select MAX(NA_NOTA) From Fnotaa_ecom Where COD_ENTID = @entidad),0)+1) ),8);

		Set @precio_cigv = @tot_precio_sigv*1.18
		Set @total_sigv = @tot_precio_sigv - @tot_dcto_sigv

		/************************************************************/
		/*******AGREGA CABECERA FACTURA******************************/
		/************************************************************/
		Insert Into Ffactc_ecom (	FC_NINT,	COD_ENTID,	FC_NNOT,	FC_CODI,	FC_SUNA,	FC_SFAC,	FC_NFAC,	FC_FFAC,	FC_CREF,	FC_SREF,	FC_NREF,	FC_NCLI,	FC_NOMB,	FC_APEP,
									FC_APEM,	FC_DCLI,	FC_RUC,		FC_VUSE,	FC_VEND,	FC_PINT,	FC_NCON,	FC_LCON,	FC_LRUC,	FC_AGEN,	FC_MONE,	FC_TASA,
									FC_FPAG,	FC_NLET,	FC_QTOT,	FC_PREF,	FC_DREF,	FC_BRUT,	FC_VIMP1,	FC_VIMP2,	FC_VDCT1,	FC_VDCT4,	FC_PDC2,	FC_PDC3,
									FC_VDC23,	FC_VVTA,	FC_VIMP3,	FC_PIMP4,	FC_VIMP4,	FC_TOTAL,	FC_CUSE,	FC_MUSE,	FC_FCRE,	FC_HORA,	FEC_SERVER) 
		Values (@codigo_fact, @entidad, @codigo_nota, @tipo_doc, @tipo_doc_sunat, @serie_doc, @numero_doc, @ven_fecha, @cod_ref, @ser_ref, @num_ref, @pri_nom_cli+' '+@seg_nom_cli+' '+@pri_ape_cli+' '+@seg_ape_cli,
				@pri_nom_cli+ ' '+@seg_nom_cli, @pri_ape_cli, @seg_ape_cli, @dir_cli, @ruc_cli, @usu_crea, @cod_vend, 0, @codigo, @telf_cli, @ruc_cli, @doc_pago, @moneda, @tipo_camb, @forma_pago,
				0, @tot_cant, @precio_cigv, @tot_dcto_sigv, @tot_precio_sigv, 0, 0, @tot_dcto_sigv, 0, 0, 0, 0, @total_sigv, @tot_igv, 0, 0, @total_venta, 'www', @usu_crea, @ven_fecha, @ven_hora, GETDATE());
	
		
		/************************************************************/
		/*******AGREGA PAGO FACTURA**********************************/
		/************************************************************/
		IF (@tipo = 'VT')
		BEGIN
			Insert Into Fnotaa_ecom(	NA_NOTA,	COD_ENTID,	NA_ITEM,	NA_MONE,	NA_TPAG,	NA_TASA,	NA_NREF,	NA_VREF,	NA_VPAG,	NA_CIER,	NA_FCRE)
			Values (@codigo_nota, @entidad, '001', @moneda, @forma_pago, @tipo_camb, @doc_pago, @total_venta, @total_venta, 'C', @ven_fecha+' '+@ven_hora+ ' VEN');
		END
		
		
		/************************************************************/
		/*******AGREGA DETALLE FACTURA*******************************/
		/************************************************************/
		Set @contador = 0;

		Declare	@articulos_det			Varchar(max),
				@tallas_det				Varchar(max),
				@cant_artic_det			Varchar(max),
				@alm_tien_det			Varchar(max),
				@prec_artic_det			Varchar(max),	-- Sin IGV Unitario (sin dcto)
				@dcto_artic_det			Varchar(max),	-- Sin IGV Total
				@seccion_det			Varchar(max),	

				@precio_unit_cigv		decimal(18,2),
				@dcto_unit_sigv			decimal(18,2),
				@precio_tot_sigv		decimal(18,2),
				@total_igv				decimal(18,2),
				@total_artic_cigv		decimal(18,2)

		DECLARE detalle_cursor CURSOR FOR
		Select Right('000'+Item,3) From UFN_SplitString(@items,'|');
		
		OPEN detalle_cursor;
		
		FETCH NEXT FROM detalle_cursor
		INTO @Item;
		
		WHILE @@FETCH_STATUS = 0
		BEGIN
			Set @contador = @contador + 1 ;
			
			--// Se obtienen datos a partir de cadenas
			exec USP_ExtraeString	@articulos,		'|',		@contador,		@articulos_det	OutPut;
			exec USP_ExtraeString	@tallas,		'|',		@contador,		@tallas_det		OutPut;
			exec USP_ExtraeString	@cant_artic,	'|',		@contador,		@cant_artic_det OutPut;
			exec USP_ExtraeString	@alm_tien,		'|',		@contador,		@alm_tien_det	OutPut;
			exec USP_ExtraeString	@prec_artic,	'|',		@contador,		@prec_artic_det OutPut;
			exec USP_ExtraeString	@dcto_artic,	'|',		@contador,		@dcto_artic_det OutPut;
			exec USP_ExtraeString	@seccion,		'|',		@contador,		@seccion_det	OutPut;

			--// Se procesan datos para calculos necesarios
			Set @precio_unit_cigv = round(Convert(decimal(18,2),@prec_artic_det)*1.18,1);
			Set @dcto_unit_sigv = round(convert(decimal(18,2),@dcto_artic_det)/convert(decimal(18,2),@cant_artic_det),1);
			Set @precio_tot_sigv = convert(decimal(18,2),@prec_artic_det)*convert(decimal(18,2),@cant_artic_det);
			Set @total_igv = round(@precio_tot_sigv*0.18,1);
			Set @total_artic_cigv = round(@precio_tot_sigv*1.18,1);


			
			Insert Into Ffactd_ecom (	FD_NINT,	COD_ENTID,	FD_TIPO,	FD_ARTI,	ID_CALIDAD,	FD_REGL,	FD_COLO,	FD_ITEM,	FD_QFAC,	FD_LPRE,	FD_CALM,
										FD_PREF,	FD_DREF,	FD_PREC,	FD_BRUT,	FD_PIMP1,	FD_VIMP1,	FD_SUBT1,	FD_PIMP2,	FD_VIMP2,	FD_SUBT2,	FD_PDCT1,	FD_VDCT1,
										FD_SUBT3,	FD_VDCT4,	FD_VDC23,	FD_VVTA,	FD_PIMP3,	FD_VIMP3,	FD_PIMP4,	FD_VIMP4,	FD_TOTAL,	FD_CUSE,	FD_MUSE,	FD_FCRE,
										COD_SECCI)
			Values (@codigo_fact, @entidad, 'A', @articulos_det, '1', @tallas_det, NULL, @Item, @cant_artic_det, '01', @alm_tien_det, @precio_unit_cigv, @dcto_unit_sigv,
					@prec_artic_det, @precio_tot_sigv, 0, 0, @precio_tot_sigv, 0, 0, @precio_tot_sigv, 0, 0, @precio_tot_sigv, 0, 0, @precio_tot_sigv, 18, @total_igv, 0, 0, @total_artic_cigv,
					NULL, @usu_crea, @ven_fecha, @seccion_det);
			
			FETCH NEXT FROM detalle_cursor
			INTO @Item;
		END

		CLOSE detalle_cursor;
		DEALLOCATE detalle_cursor;

		COMMIT TRAN ins_Venta
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN ins_Venta
		
		CLOSE detalle_cursor;
		DEALLOCATE detalle_cursor;

		DECLARE		@ErrorMessage	NVARCHAR(4000),
					@ErrorSeverity	INT,
					@ErrorState		INT; 

		SELECT		@ErrorMessage = ERROR_MESSAGE(),
					@ErrorSeverity = ERROR_SEVERITY(),
					@ErrorState = ERROR_STATE(); 

		RAISERROR (	@ErrorMessage, @ErrorSeverity, @ErrorState ); 

	END CATCH
END
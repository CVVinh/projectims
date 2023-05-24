<template>
  <Dialog v-model:visible="this.display" :closable="false" modal>
    <template #header>
		<h4>Xóa thành viên ra khỏi dự án ?</h4>
	</template>
	Bạn có chắc chắn xóa thành viên này ra khỏi nhóm ?

	<template #footer>
		<Button label="Hủy" class="p-button-secondary p-button-outlined aloha CustomButtonPrimeVue" autofocus icon="pi pi-times" @click="this.$emit('close')"/>
        <Button label="Xóa" class="p-button-danger CustomButtonPrimeVue" autofocus icon="pi pi-trash"  @click="this.$emit('delete')"/>
	</template>
</Dialog>
</template>

<script>
import ButtonCustom from "@/components/buttons/ButtonCustom.vue"

export default {
	components: { ButtonCustom },
    props:['display']
}
  

</script>

<style>

</style>